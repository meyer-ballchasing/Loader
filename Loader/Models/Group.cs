﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meyer.BallChasing.Models
{
    public class Group
    {
        public string BallChasingId { get; set; }

        public string Name { get; set; }

        public string PlayerIdentification { get; set; } = "by-id";

        public string TeamIdentification { get; set; } = "by-distinct-players";

        public Group Parent { get; set; }

        public List<Group> Children { get; set; }

        public List<Replay> Replays { get; set; }

        public Group()
        {
            this.Children = new List<Group>(0);
            this.Replays = new List<Replay>(0);
        }

        public Group(DirectoryInfo directory, Group parent)
        {
            this.Name = directory.Name;

            this.Children = directory
                .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Select(x => new Group(x, this))
                .ToList();

            this.Replays = directory
                .EnumerateFiles("*.replay", SearchOption.TopDirectoryOnly)
                .Select(x => new Replay
                {
                    Group = this,
                    LocalFile = x,
                    Public = true
                })
                .ToList();

            if (parent != null)
                this.Parent = parent;
        }

        public Group Find(Group group)
        {
            if (group == null)
                return null;

            if (group.Equals(this))
                return this;

            foreach (var child in this.Children)
            {
                var found = child.Find(group);
                if (found != null)
                    return found;
            }

            return null;
        }

        public void Merge(Group group)
        {
            if (group == null)
                return;

            var found = group.Find(this);

            if (found != null)
            {
                this.BallChasingId = found.BallChasingId;

                this.Replays = this.Replays.Select(x =>
                {
                    Replay match = found.Replays.SingleOrDefault(y => y.Equals(x));

                    if (match != null && match.Group.Equals(this))
                        return match;

                    return x;
                })
                .ToList();
            }

            foreach (var child in this.Children)
                child.Merge(group);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Group compare = obj as Group;

            return compare != null
                && ((compare.BallChasingId != null && this.BallChasingId != null && compare.BallChasingId == this.BallChasingId) || compare.Name == this.Name)
                && ((compare.Parent == null && this.Parent == null) || (compare.Parent != null && this.Parent != null && compare.Parent.Equals(this.Parent)));
        }
    }
}