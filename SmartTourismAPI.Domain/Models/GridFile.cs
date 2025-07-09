using SmartTourismAPI.Domain.Models.Geometries;

namespace SmartTourismAPI.Domain.Models;

public class GridFile {
    public List<double> Sx { get; }
    public List<double> Sy { get; }

    private readonly Dictionary<int, List<Entry>> pages;
    private readonly Dictionary<(int, int), int> directory;
    private readonly int pageCapacity;
    private int nextPageId = 1;
    private int nextObjectId = 1;

    public GridFile(double minX, double maxX, double minY, double maxY, int pageCapacity) {
        Sx = [minX, maxX];
        Sy = [minY, maxY];
        directory = [];
        pages = [];
        this.pageCapacity = pageCapacity;
    }

    public void Insert(string osmId, Point p) {
        int i = Rank(p.X, Sx);
        int j = Rank(p.Y, Sy);

        if (!directory.ContainsKey((i, j))) {
            directory[(i, j)] = nextPageId++;
        }

        int pageId = directory[(i, j)];
        if (!pages.ContainsKey(pageId)) {
            pages[pageId] = [];
        }

        var page = pages[pageId];
        page.Add(new Entry(nextObjectId++, osmId, p));

        if (page.Count > pageCapacity) {
            bool isReferencedByMultipleCells = directory.Values.Count(v => v == pageId) > 1;
            if (isReferencedByMultipleCells) {
                AllocateNewPages(i, j);
            } else {
                SplitCellsAndAllocateNewPages(i, j);
            }
        }
    }

    private void SplitCellsAndAllocateNewPages(int i, int j) {
        double deltaX = Sx[i + 1] - Sx[i];
        double deltaY = Sy[j + 1] - Sy[j];

        if (deltaX >= deltaY) {
            for (int m = 0; m < Sy.Count - 1; m++) {
                for (int n = Sx.Count - 2; n >= i + 1; n--) {
                    directory[(n + 1, m)] = directory[(n, m)];
                }
            }

            double midX = (Sx[i] + Sx[i + 1]) / 2;
            Sx.Insert(i + 1, midX);

            int oldPageId = directory[(i, j)];

            for (int k = 0; k < Sy.Count - 1; k++) {
                if (k != j) {
                    directory[(i + 1, k)] = directory[(i, k)];
                }
            }

            for (int k = 0; k < Sy.Count - 1; k++) {
                if (k == j) {
                    int newPageId = nextPageId++;
                    directory[(i + 1, j)] = newPageId;

                    List<Entry> oldPage = pages[oldPageId];
                    pages[oldPageId] = [];
                    pages[newPageId] = [];

                    RedistributeEntries(oldPage);
                }
            }
        } else {
            for (int n = 0; n < Sx.Count - 1; n++) {
                for (int m = Sy.Count - 2; m >= j + 1; m--) {
                    directory[(n, m + 1)] = directory[(n, m)];
                }
            }

            double midY = (Sy[j] + Sy[j + 1]) / 2;
            Sy.Insert(j + 1, midY);

            int oldPageId = directory[(i, j)];

            for (int k = 0; k < Sx.Count - 1; k++) {
                if (k != i) {
                    directory[(k, j + 1)] = directory[(k, j)];
                }
            }

            for (int k = 0; k < Sx.Count - 1; k++) {
                if (k == i) {
                    int newPageId = nextPageId++;
                    directory[(i, j + 1)] = newPageId;

                    List<Entry> oldPage = pages[oldPageId];
                    pages[oldPageId] = [];
                    pages[newPageId] = [];

                    RedistributeEntries(oldPage);
                }
            }
        }
    }

    private void AllocateNewPages(int i, int j) {
        int oldPageId = directory[(i, j)];
        int newPageId = nextPageId++;

        directory[(i, j)] = newPageId;

        List<Entry> oldPage = pages[oldPageId];
        pages[oldPageId] = [];
        pages[newPageId] = [];

        RedistributeEntries(oldPage);
    }

    private void RedistributeEntries(List<Entry> entries) {
        foreach (var e in entries) {
            int i = Rank(e.Point.X, Sx);
            int j = Rank(e.Point.Y, Sy);

            if (!directory.ContainsKey((i, j))) {
                int newPageId = nextPageId++;
                directory[(i, j)] = newPageId;
                pages[newPageId] = [];
            }

            int pageId = directory[(i, j)];
            pages[pageId].Add(e);

            if (pages[pageId].Count > pageCapacity) {
                bool isReferencedByMultipleCells = directory.Values.Count(v => v == pageId) > 1;
                if (isReferencedByMultipleCells) {
                    AllocateNewPages(i, j);
                } else {
                    SplitCellsAndAllocateNewPages(i, j);
                }
            }
        }
    }

    private static int Rank(double value, List<double> scale) {
        int index = scale.BinarySearch(value);
        return index >= 0 ? index : ~index - 1;
    }

    public List<Entry> GetEntries() {
        var result = new List<Entry>();
        foreach (var page in pages.Values) {
            result.AddRange(page);
        }
        return result;
    }

    public void Clear() {
        pages.Clear();
        directory.Clear();
        nextPageId = 1;
        nextObjectId = 1;

        Sx.RemoveRange(1, Sx.Count - 2);
        Sy.RemoveRange(1, Sy.Count - 2);
    }

    #region Queries

    public List<Entry> RadiusQuery(Circle circle) {
        List<Entry> result = [];

        int i1 = Rank(circle.Center.X - circle.Radius, Sx);
        int j1 = Rank(circle.Center.Y - circle.Radius, Sy);
        int i2 = Rank(circle.Center.X + circle.Radius, Sx);
        int j2 = Rank(circle.Center.Y + circle.Radius, Sy);

        var visitedPages = new HashSet<int>();
        for (int i = i1; i <= i2; i++) {
            for (int j = j1; j <= j2; j++) {
                if (directory.ContainsKey((i, j))) {
                    int pageId = directory[(i, j)];
                    if (!visitedPages.Contains(pageId) && pages.TryGetValue(pageId, out List<Entry>? value)) {
                        result.AddRange(value.Where(e => circle.Contains3D(e.Point)));
                        visitedPages.Add(pageId);
                    }
                }
            }
        }

        return result;
    }

    #endregion
}