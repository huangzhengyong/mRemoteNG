﻿using mRemoteNG.Connection;
using mRemoteNG.Container;
using NUnit.Framework;


namespace mRemoteNGTests.Container
{
    public class ContainerInfoTests
    {
        private ContainerInfo _containerInfo;
        private ConnectionInfo _con1;
        private ConnectionInfo _con2;
        private ConnectionInfo _con3;

        [SetUp]
        public void Setup()
        {
            _containerInfo = new ContainerInfo();
            _con1 = new ConnectionInfo {Name = "a"};
            _con2 = new ConnectionInfo {Name = "b"};
            _con3 = new ConnectionInfo {Name = "c"};
        }

        [TearDown]
        public void Teardown()
        {
            _containerInfo = null;
            _con1 = null;
            _con2 = null;
            _con3 = null;
        }

        [Test]
        public void AddSetsParentPropertyOnTheChild()
        {
            _containerInfo.AddChild(_con1);
            Assert.That(_con1.Parent, Is.EqualTo(_containerInfo));
        }

        [Test]
        public void AddAddsChildToChildrenList()
        {
            _containerInfo.AddChild(_con1);
            Assert.That(_containerInfo.Children, Does.Contain(_con1));
        }

        [Test]
        public void AddRangeAddsAllItems()
        {
            var collection = new[] { _con1, _con2, _con3 };
            _containerInfo.AddChildRange(collection);
            Assert.That(_containerInfo.Children, Is.EquivalentTo(collection));
        }

        [Test]
        public void RemoveUnsetsParentPropertyOnChild()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.RemoveChild(_con1);
            Assert.That(_con1.Parent, Is.Not.EqualTo(_containerInfo));
        }

        [Test]
        public void RemoveRemovesChildFromChildrenList()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.RemoveChild(_con1);
            Assert.That(_containerInfo.Children, Does.Not.Contains(_con1));
        }

        [Test]
        public void RemoveRangeRemovesAllIndicatedItems()
        {
            var collection = new[] { _con1, _con2, new ContainerInfo() };
            _containerInfo.AddChildRange(collection);
            _containerInfo.RemoveChildRange(collection);
            Assert.That(_containerInfo.Children, Does.Not.Contains(collection[0]).And.Not.Contains(collection[1]).And.Not.Contains(collection[2]));
        }

        [Test]
        public void RemoveRangeDoesNotRemoveUntargetedMembers()
        {
            var collection = new[] { _con1, _con2, new ContainerInfo() };
            _containerInfo.AddChildRange(collection);
            _containerInfo.AddChild(_con3);
            _containerInfo.RemoveChildRange(collection);
            Assert.That(_containerInfo.Children, Does.Contain(_con3));
        }

        [Test]
        public void AddingChildTriggersCollectionChangedEvent()
        {
            var wasCalled = false;
            _containerInfo.CollectionChanged += (sender, args) => wasCalled = true;
            _containerInfo.AddChild(_con1);
            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void RemovingChildTriggersCollectionChangedEvent()
        {
            var wasCalled = false;
            _containerInfo.AddChild(_con1);
            _containerInfo.CollectionChanged += (sender, args) => wasCalled = true;
            _containerInfo.RemoveChild(_con1);
            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void ChangingChildPropertyTriggersPropertyChangedEvent()
        {
            var wasCalled = false;
            _containerInfo.AddChild(_con1);
            _containerInfo.PropertyChanged += (sender, args) => wasCalled = true;
            _con1.Name = "somethinghere";
            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void ChangingSubChildPropertyTriggersPropertyChangedEvent()
        {
            var wasCalled = false;
            var container2 = new ContainerInfo();
            _containerInfo.AddChild(container2);
            container2.AddChild(_con1);
            _containerInfo.PropertyChanged += (sender, args) => wasCalled = true;
            _con1.Name = "somethinghere";
            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void SetChildPositionPutsChildInCorrectPosition()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            _containerInfo.SetChildPosition(_con2, 2);
            Assert.That(_containerInfo.Children.IndexOf(_con2), Is.EqualTo(2));
        }

        [Test]
        public void SettingChildPositionAboveArrayBoundsPutsItAtEndOfList()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var finalIndex = _containerInfo.Children.Count - 1;
            _containerInfo.SetChildPosition(_con2, 5);
            Assert.That(_containerInfo.Children.IndexOf(_con2), Is.EqualTo(finalIndex));
        }

        [Test]
        public void SettingChildPositionBelowArrayBoundsDoesNotMoveTheChild()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var originalIndex = _containerInfo.Children.IndexOf(_con2);
            _containerInfo.SetChildPosition(_con2, -1);
            Assert.That(_containerInfo.Children.IndexOf(_con2), Is.EqualTo(originalIndex));
        }

        [Test]
        public void SetChildAbovePutsChildInCorrectPosition()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var referenceChildIndexBeforeMove = _containerInfo.Children.IndexOf(_con2);
            _containerInfo.SetChildAbove(_con3, _con2);
            var targetsNewIndex = _containerInfo.Children.IndexOf(_con3);
            Assert.That(targetsNewIndex, Is.EqualTo(referenceChildIndexBeforeMove));
        }

        [Test]
        public void SetChildBelowPutsChildInCorrectPosition()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var referenceChildIndexBeforeMove = _containerInfo.Children.IndexOf(_con1);
            _containerInfo.SetChildBelow(_con3, _con1);
            var targetsNewIndex = _containerInfo.Children.IndexOf(_con3);
            Assert.That(targetsNewIndex, Is.EqualTo(referenceChildIndexBeforeMove+1));
        }

        [Test]
        public void PromoteChildMovesTargetUpOne()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var targetsIndexBeforeMove = _containerInfo.Children.IndexOf(_con3);
            _containerInfo.PromoteChild(_con3);
            var targetsNewIndex = _containerInfo.Children.IndexOf(_con3);
            Assert.That(targetsNewIndex, Is.EqualTo(targetsIndexBeforeMove - 1));
        }

        [Test]
        public void PromoteChildDoesNothingWhenAlreadyAtTopOfList()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var targetsIndexBeforeMove = _containerInfo.Children.IndexOf(_con1);
            _containerInfo.PromoteChild(_con1);
            var targetsNewIndex = _containerInfo.Children.IndexOf(_con1);
            Assert.That(targetsNewIndex, Is.EqualTo(targetsIndexBeforeMove));
        }

        [Test]
        public void DemoteChildMovesTargetDownOne()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var targetsIndexBeforeMove = _containerInfo.Children.IndexOf(_con1);
            _containerInfo.DemoteChild(_con1);
            var targetsNewIndex = _containerInfo.Children.IndexOf(_con1);
            Assert.That(targetsNewIndex, Is.EqualTo(targetsIndexBeforeMove + 1));
        }

        [Test]
        public void DemoteChildDoesNothingWhenAlreadyAtTopOfList()
        {
            _containerInfo.AddChild(_con1);
            _containerInfo.AddChild(_con2);
            _containerInfo.AddChild(_con3);
            var targetsIndexBeforeMove = _containerInfo.Children.IndexOf(_con3);
            _containerInfo.DemoteChild(_con3);
            var targetsNewIndex = _containerInfo.Children.IndexOf(_con3);
            Assert.That(targetsNewIndex, Is.EqualTo(targetsIndexBeforeMove));
        }
    }
}