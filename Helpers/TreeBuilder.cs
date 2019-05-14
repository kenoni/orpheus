using System.Collections.Generic;
using System.Linq;
using Orpheus.Models;

namespace Orpheus.Helpers
{
    public class TreeBuilder<T>
    {
        private readonly Dictionary<int,ITreeItem<T>> _items;
        private readonly char _delimiter;

        public TreeBuilder(Dictionary<int,ITreeItem<T>> items, char delimiter)
        {
            _items = items;
            _delimiter = delimiter;
        }

        public Dictionary<int,ITreeItem<T>> BuildTree()
        {
            var roots = _items.Where(x => !x.Value.Uri.Contains(_delimiter)).ToDictionary(x => x.Key, x => x.Value);

            if (roots.Count <= 0) return roots;

            foreach (var child in roots)
                AddChildren(child.Value);



            return roots;
        }

        private void AddChildren(ITreeItem<T> treeItem)
        {
            if (_items.Any(x => x.Value.Uri.StartsWith(treeItem.Uri)))
            {
                var slashCount = treeItem.Uri.Count(x => x == _delimiter);
                treeItem.Children = _items.Where(x => x.Value.Uri.StartsWith(treeItem.Uri + _delimiter) 
                                                        && slashCount + 1 == x.Value.Uri.Count(x1 => x1 == _delimiter)).ToDictionary(x => x.Key,x => x.Value);

                foreach (var child in treeItem.Children)
                {
                    if (!string.IsNullOrEmpty(treeItem.Uri) 
                        && child.Value.Name.StartsWith($"{treeItem.Uri}/") 
                        && child.Value.Type == MpdFileType.Folder 
                        && treeItem.Type == MpdFileType.Folder)
                    {
                        child.Value.Name = child.Value.Name.Substring(treeItem.Name.Length + 1);
                    }

                    AddChildren(child.Value);
                }
            }
            else
            {
                treeItem.Children = new Dictionary<int, ITreeItem<T>>();
            }
        }
    }
}