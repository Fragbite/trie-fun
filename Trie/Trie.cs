using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trie
{
    public class Trie
    {
        private TrieLeaf _rootNode;

        public Trie(List<string> words)
        {
            this.BuildTrie(words);
        }

        private void BuildTrie(List<string> words)
        {
            _rootNode = new TrieLeaf();
            _rootNode.Value = null;
            _rootNode.Parent = null;
            _rootNode.Index = 0;
            _rootNode.Id = Guid.Empty;

            foreach (var word in words)
            {
                AddWordToTrie(word);
            }
        }

        private void AddWordToTrie(string word)
        {
            var existingNode = _rootNode.Leafs.FirstOrDefault(arg => word.StartsWith(arg.Value));

            if (existingNode == null)
            {
                var leafNodes = BuildLeafs(word, _rootNode.Index + 1, _rootNode);
                _rootNode.Leafs.AddRange(leafNodes);
            }
            else 
            {
                var leafNodes = BuildLeafs(word, _rootNode.Index + 1, existingNode);
                if (leafNodes.Any())
                {
                    existingNode.MergeNodes(leafNodes.First());
                }
            }
        }
        
        private static List<TrieLeaf> BuildLeafs(string text, int index, TrieLeaf parent)
        {
            var leafs = new List<TrieLeaf>();

            if (text.Length >= index)
            {
                var leaf = new TrieLeaf();
                leaf.Value = text.Substring(0, index);
                leaf.Index = index;
                leaf.Parent = parent;
                leaf.Id = Guid.NewGuid();
                leaf.Leafs = BuildLeafs(text, index + 1, leaf);
                leafs.Add(leaf);
            }

            return leafs;
        }

        public void Clear()
        {
            _rootNode = null;
            Console.WriteLine("Trie cleared.");
        }

        public void OrderTree()
        {
            _rootNode.OrderNodes();
        }

        public void PrintTree()
        {
            _rootNode.PrintNode("", Console.Out);
        }

        public void SaveToFile(string fileName)
        {
            using (var file = System.IO.File.CreateText(fileName))
            {
                _rootNode.PrintNode("", file);
            }

            Console.WriteLine("Tree saved to file {0}", fileName);
        }

        public void Find(string value)
        {
            var found = _rootNode.FindNode(value);
            if (!found)
            {
                Console.WriteLine("Value {0} could not be found in the tree", value);
            }
            else
            {
                Console.Write(" root node");
                Console.WriteLine();
            }
        }

        public void FindById(string value)
        {
            Guid id;
            if (!Guid.TryParse(value, out id))
            {
                Console.WriteLine("{0} is not a valid id (should be guid)", value);
            }
            var found = _rootNode.FindNodeById(id);
            if (!found)
            {
                Console.WriteLine("Value {0} could not be found in the tree", value);
            }
            else
            {
                Console.Write(" root node");
                Console.WriteLine();
            }
        }

        public void AddValue(string value)
        {
            var found = _rootNode.FindNode(value, false);
            if (found)
            {
                Console.WriteLine("Value already exists");
            }
            else
            {
                AddWordToTrie(value);
                Console.WriteLine("Value added");
            }
        }

        public void PrintStats()
        {
            var deepestLevel = _rootNode.FindDeepestLevel();
            Console.WriteLine("Trie levels: {0}", deepestLevel);
            for (int i = 0; i < deepestLevel; i++)
            {
                var levelCount = _rootNode.CountByLevel(i);
                Console.WriteLine("Level {0} nodes : {1}", i, levelCount);
            }
        }
    }
}
