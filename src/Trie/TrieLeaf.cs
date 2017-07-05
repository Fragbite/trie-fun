using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trie
{
    public class TrieLeaf
    {
        public string Value { get; set; }

        public int Index { get; set; }

        public Guid Id { get; set; }

        public List<TrieLeaf> Leafs { get; set; }

        public TrieLeaf Parent { get; set; }

        public TrieLeaf()
        {
            Leafs = new List<TrieLeaf>();
        }

        public TrieLeaf this[int i]{
            get
            {
                return Leafs[i];
            }
            set
            {
                Leafs[i] = value;
            }
        }

        public int CountByLevel(int level)
        {
            if (Index == level)
            {
                return 1;
            }
            var count = 0;
            foreach (var leaf in Leafs)
            {
                count += leaf.CountByLevel(level);
            }
            return count;
        }

        public int FindDeepestLevel()
        {
            if (!Leafs.Any())
                return Index;

            var leafsLevels = new List<int>();
            foreach (var leaf in Leafs)
            {
                leafsLevels.Add(leaf.FindDeepestLevel());
            }
            return leafsLevels.Max();
        }

        public bool FindNode(string value, bool verbose = true)
        {
            if (value.Equals(Value, StringComparison.InvariantCultureIgnoreCase))
            {
                if (verbose)
                {
                    Console.WriteLine("String: {0} found at level {1}", value, Index);
                    Console.Write("Chain: ");
                }
                return true;
            }
            if (string.IsNullOrEmpty(Value) || value.StartsWith(Value, StringComparison.InvariantCultureIgnoreCase))
            {
                bool found = false;
                foreach (var leaf in Leafs)
                {
                    if (leaf.FindNode(value, verbose))
                    {
                        if (verbose)
                        {
                            Console.Write(" {0} <=", leaf.Value);
                        }
                        found = true;
                    }
                }
                return found;
            }
            return false;
        }

        public bool FindNodeById(Guid id, bool verbose = true)
        {
            if (id == this.Id)
            {
                if (verbose)
                {
                    Console.WriteLine("Node id {0} found at level {1}", id, Index);
                    Console.Write("Chain: ");
                }
                return true;
            }
            bool found = false;
            foreach (var leaf in Leafs)
            {
                if (leaf.FindNodeById(id, verbose))
                {
                    if (verbose)
                    {
                        Console.Write(" {0} <=", leaf.Value);
                    }
                    found = true;
                }
            }
            return found;
        }

        public void OrderNodes()
        {
            Leafs.ForEach(leaf => leaf.OrderNodes());
            Leafs = Leafs.OrderBy(arg => arg.Value).ToList();
        }
        
        public void PrintNode(string prefix, TextWriter textWriter)
        {
            if (Index == 0)
            {
                textWriter.WriteLine();
            }

            textWriter.WriteLine("{0} lvl:{1} {2} : {3}", prefix, this.Index, this.Id.ToString("B"), this.Value);

            foreach (TrieLeaf n in Leafs)
                n.PrintNode(prefix + "   |", textWriter);
        }

        public void MergeNodes(TrieLeaf newleaf)
        {
            if (newleaf != null)
            {
                foreach (var leaf in newleaf.Leafs)
                {
                    var existing = Leafs.FirstOrDefault(arg => leaf.Value == arg.Value);
                    if (existing != null)
                    {
                        existing.MergeNodes(leaf);
                    }
                    else
                    {
                        Leafs.Add(leaf);
                    }
                }
            }
        }
    }
}
