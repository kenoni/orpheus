using System.Collections.Generic;
using System.Linq;
using Orpheus.Models;

namespace Orpheus.Helpers
{
    public class TreeBuilder<T>
    {
        private readonly IList<ITreeItem<T>> _items;
        private readonly char _delimiter;

        public TreeBuilder(IList<ITreeItem<T>> items, char delimiter)
        {
            _items = items;
            _delimiter = delimiter;
        }

        public IList<ITreeItem<T>> BuildTree()
        {
            var roots = _items.Where(x => !x.Uri.Contains(_delimiter)).ToList();

            if (roots.Count <= 0) return roots;

            foreach (var child in roots)
                AddChildren(child);

            return roots;
        }

        private void AddChildren(ITreeItem<T> treeItem)
        {
            if (_items.Any(x => x.Uri.StartsWith(treeItem.Uri)))
            {
                var slashCount = treeItem.Uri.Count(x => x == _delimiter);
                treeItem.Children = _items.Where(x => x.Uri.StartsWith(treeItem.Uri + _delimiter) && slashCount + 1 == x.Uri.Count(x1 => x1 == _delimiter)).ToList();

                foreach (var child in treeItem.Children)
                    AddChildren(child);
            }
            else
            {
                treeItem.Children = new List<ITreeItem<T>>();
            }
        }
    }
}