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

        private void AddChildren(ITreeItem<T> mpdFile)
        {
            if (_items.Any(x => x.Uri.StartsWith(mpdFile.Uri)))
            {
                var slashCount = mpdFile.Uri.Count(x => x == _delimiter);
                mpdFile.Children = _items.Where(x => x.Uri.StartsWith(mpdFile.Uri + _delimiter) && slashCount + 1 == x.Uri.Count(x1 => x1 == _delimiter)).ToList();

                foreach (var child in mpdFile.Children)
                    AddChildren(child);
            }
            else
            {
                mpdFile.Children = new List<ITreeItem<T>>();
            }
        }
    }
}