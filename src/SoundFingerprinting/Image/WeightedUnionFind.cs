namespace SoundFingerprinting.Image
{
    using System.Collections.Generic;
    using System.Linq;

    public class WeightedUnionFind
    {
        private readonly int[] id;
        private readonly int[] sz;
        private readonly int width;
        private readonly int height;

        private int count;

        public WeightedUnionFind(int width, int height)
        {
            this.width = width;
            this.height = height;

            int n = width * height;
            id = new int[n];
            for (int i = 0; i < n; i++)
                id[i] = i;
            sz = new int[n];
            for (int i = 0; i < n; i++)
                sz[i] = 1;

            count = n;
        }

        public IEnumerable<Contour> FindDisjointContours(int threshold)
        {
            return Enumerable.Range(0, id.Length)
                .Select(coord => new { parent = Find(coord), coord })
                .GroupBy(x => x.parent)
                .Select(a =>
                {
                    var all = a.Select(i => i.coord).ToList();
                    return new Contour(Resolve(all.Min()), Resolve(all.Max()), all.Count);
                })
                .Where(x => x.Area >= threshold)
                .ToList();
        }

        public void Union(int p, int q)
        {
            int rp = Find(p);
            int rq = Find(q);
            if (rp == rq)
                return;
            if (sz[p] <= sz[q])
            {
                id[rp] = rq;
                sz[q] += sz[p];
            }
            else
            {
                id[rq] = rp;
                sz[p] += sz[q];
            }
            count--;
        }

        public bool Connected(int p, int q)
        {
            return Find(p) == Find(q);
        }

        public int GetCount()
        {
            return count;
        }
        
        private int Find(int p)
        {
            int root = p;
            while (root != id[root])
                root = id[root];
            while (p != root)
            {
                int newp = id[p];
                id[p] = root;
                p = newp;
            }

            return root;
        }
        
        private Coord Resolve(int index)
        {
            int y = index / width;
            int x = index % width;
            return new Coord(x, y);
        }
    }
}