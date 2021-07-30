using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Quark
{
    [Serializable]
    public struct QuarkDirHashPair:IEquatable<QuarkDirHashPair>
    {
        public string Dir;
        public string Hash;
        public QuarkDirHashPair(string hash, string dir)
        {
            Hash= hash;
            Dir = dir;
        }
        public static bool operator ==(QuarkDirHashPair a, QuarkDirHashPair b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(QuarkDirHashPair a, QuarkDirHashPair b)
        {
            return !a.Equals(b);
        }
        public override bool Equals(object obj)
        {
            return obj is QuarkDirHashPair && Equals((QuarkDirHashPair)obj);
        }
        public override int GetHashCode()
        {
            return Hash.GetHashCode() ^ Dir.GetHashCode();
        }
        public override string ToString()
        {
            return $"Hash: {(string.IsNullOrEmpty(Hash)==true?"Null":Hash)} ; Dir :{(string.IsNullOrEmpty(Dir)==true?"Null":Dir)}";
        }
        public bool Equals(QuarkDirHashPair other)
        {
            return other.Hash == this.Hash && other.Dir== this.Dir;
        }
        public static QuarkDirHashPair None { get { return new QuarkDirHashPair(); } }
    }
}
