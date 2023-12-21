string inputFile = @"C:\Users\SaLiVa\source\repos\AdventOfCode2023Day10\AdventOfCode2023Day10\Day10Input.txt";
string outputFile = @"C:\Users\SaLiVa\source\repos\AdventOfCode2023Day10\AdventOfCode2023Day10\Day10Output.txt";
List<MapNode> map = [];

using (StreamReader reader = File.OpenText(inputFile))
{
    int x = 0, y = 0;
    while (!reader.EndOfStream)
    {
        var mapChar = reader.Read();
        switch (mapChar)
        {
            case '\r':
                x = 0;
                break;
            case '\n':
                x = 0;
                y++;
                break;
            default:
                map.Add(new MapNode(x, y, (char)mapChar));
                x++;
                break;
        }
    }
}

/* This is Part 1 */

// Find the start node
var startNode = map.First(x => x.NodeType == 'S');
var pipeMap = new List<MapNode> ();
var steps = 0;
var currentNode = startNode;
var XMaxLimit = map.Max(x => x.X);
var YMaxLimit = map.Max(y => y.Y);

while (startNode.NumberOfVisits <= 1)
{
    MapNode? nextNode = null;
    
    MapNode? northNode = null;
    MapNode? southNode = null;
    MapNode? westNode = null;
    MapNode? eastNode = null;
    
    // Register node as visited
    pipeMap.Add(currentNode);
    currentNode.TriggerVisit();
    
    // Scan the map to find accessible paths around the current node
    if (currentNode.Y > 0)
    {
        var nNode = map.SingleOrDefault(x => x.X == currentNode.X && x.Y == currentNode.Y - 1);
        if(nNode != null && currentNode.NorthNodeType.Contains(nNode.NodeType))
            northNode = map.Single(x => x.X == currentNode.X && x.Y == currentNode.Y - 1);
    }
    
    if(currentNode.Y < YMaxLimit)
    {
        var sNode = map.SingleOrDefault(x => x.X == currentNode.X && x.Y == currentNode.Y + 1);
        if(sNode != null && currentNode.SouthNodeType.Contains(sNode.NodeType))
            southNode = map.Single(x => x.X == currentNode.X && x.Y == currentNode.Y + 1);
    }

    if (currentNode.X < XMaxLimit)
    {
        var eNode = map.SingleOrDefault(x => x.X == currentNode.X + 1 && x.Y == currentNode.Y);
        if(eNode != null && currentNode.EastNodeType.Contains(eNode.NodeType))
            eastNode = map.Single(x => x.X == currentNode.X + 1 && x.Y == currentNode.Y);
    }

    if (currentNode.X > 0)
    {
        var wNode = map.SingleOrDefault(x => x.X == currentNode.X - 1 && x.Y == currentNode.Y);
        if(wNode != null && currentNode.WestNodeType.Contains(wNode.NodeType))
            westNode = map.Single(x => x.X == currentNode.X - 1 && x.Y == currentNode.Y);
    }
    
    // Traverse through the pipes and count the steps
    currentNode.StepsFromStart = steps;
    steps++;
    
    // Find the least travelled node and go to it
    var visitlist = new[]
        { northNode, southNode, westNode, eastNode };
    nextNode = visitlist.FirstOrDefault(x => x is { NumberOfVisits: 0 });
    
    if (nextNode == null)
        // Success scenario reached
        break;

    currentNode = nextNode;
}

var backstep = 0;
// Now go backwards through the dictionary and recalculate the steps
for (var i = pipeMap.Count - 1; i >= 0; i--)
{
    backstep++;
    pipeMap.ElementAt(i).StepsFromStart = pipeMap.ElementAt(i).StepsFromStart < backstep
        ? pipeMap.ElementAt(i).StepsFromStart
        : backstep;
}

Console.WriteLine("Furthest steps from start: " + pipeMap.Max(x=> x.StepsFromStart));

/* This is part II */

// Using Shoelace formula to calculate the Area and Pick's Theorem to calculate the number of I's
var clockwise = DetermineClockwise(pipeMap);
var area = 0.0d;

for (int i = 0; i < pipeMap.Count; i++)
{
    if (i == pipeMap.Count - 1)
    {
        area += (pipeMap[i].X * pipeMap[0].Y - pipeMap[i].Y * pipeMap[0].X);
    }
    else
    {
        area += (pipeMap[i].X * pipeMap[i + 1].Y - pipeMap[i].Y * pipeMap[i + 1].X);
    }
}

area = 0.5 * double.Abs(area);

var internalPoints = area - pipeMap.Count / 2.0d + 1;
Console.WriteLine("Area (A): " + area);
Console.WriteLine("Number of Internal Points (I): " + internalPoints);


// // Writer for sanity check
// using (StreamWriter outputWriter = new StreamWriter(outputFile))
// {
//     for (int y = 0; y <= YMaxLimit; y++)
//     {
//         string outputString = string.Empty;
//         for (int x = 0; x <= XMaxLimit; x++)
//         {
//             var nodeType = pipeMap.FirstOrDefault(p => p.X == x && p.Y == y);
//             char nodeTypeChar = ' ';
//             if(nodeType != null)
//                 nodeTypeChar = nodeType.NodeType;
//             // outputString += nodeTypeChar == '.' ? ' ' : nodeTypeChar;
//             outputString += nodeTypeChar;
//         }
//         outputWriter.WriteLine(outputString);
//     }
// }

bool DetermineClockwise(IReadOnlyList<MapNode> nodes)
{
    long total = 0;
    for (var i = 0; i < nodes.Count - 1; i++)
    {
        var n1 = nodes[i];
        var n2 = nodes[i + 1];

        total += (n2.X - n1.X) * (n2.Y + n1.Y);
    }
    
    // -ve means clock wise, +ve is counter clock wise
    return total < 0;
}

public class MapNode
{
    public MapNode(int x, int y, char nodeType)
    {
        X = x;
        Y = y;
        NodeType = nodeType;
        NumberOfVisits = 0;
        StepsFromStart = 0;
    }

    public int X { get; }
    public int Y { get; }
    public char NodeType { get; private set; }

    public void TriggerVisit()
    {
        NumberOfVisits++;
    }

    public void SetAsO()
    {
        NodeType = 'O';
    }

    public void SetAsI()
    {
        NodeType = 'I';
    }
    
    public List<char> EastNodeType 
    {
        get
        {
            switch (NodeType)
            {
                case 'S': case '-': case 'F': case 'L': return ['-', 'J', '7', 'S'];
                default: return [];
            }
        } 
    }
    
    public List<char> WestNodeType 
    { 
        get
        {
            switch (NodeType)
            {
                case 'S': case '-': case 'J': case '7': return ['-', 'F', 'L', 'S'];
                default: return [];
            }
        }  
    }

    public List<char> NorthNodeType
    {
        get
        {
            switch (NodeType)
            {
                case 'S': case '|': case 'J': case 'L': return ['|', 'F', '7', 'S'];
                default: return [];
            }
        }
    }

    public List<char> SouthNodeType 
    { 
        get
        {
            switch (NodeType)
            {
                case 'S': case '|': case 'F': case '7': return ['|', 'J', 'L', 'S'];
                default: return [];
            }
        }  
    }
    
    public int NumberOfVisits { get; private set; }
    public int StepsFromStart { get; set; }
}

/* While this implementation was fun and long winded - Alas it did not work 
// Find dot clusters (I guess)

// Find everything that is outside of the pipes

var outsidePipeMap = map.Except(pipeMap).ToList();

// New idea - When walking clockwise along the pipe - The dots on the right are the I's
// Equally walking anti-clockwise along the pipe - The dots on the left are the I's
var clockwise = DetermineClockwise(pipeMap);

Console.WriteLine(clockwise ? "Clockwise" : "Anti-clockwise");

for (int i = 1; i < pipeMap.Count; i++)
{
    var northDot = outsidePipeMap.FirstOrDefault(x => x.X == pipeMap[i].X && x.Y == pipeMap[i].Y - 1);
    var southDot = outsidePipeMap.FirstOrDefault(x => x.X == pipeMap[i].X && x.Y == pipeMap[i].Y + 1);
    var westDot = outsidePipeMap.FirstOrDefault(x => x.X == pipeMap[i].X - 1 && x.Y == pipeMap[i].Y);
    var eastDot = outsidePipeMap.FirstOrDefault(x => x.X == pipeMap[i].X + 1 && x.Y == pipeMap[i].Y);
    
    if (pipeMap[i].NodeType == '-')
    {
        if (northDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], northDot);
            if (!clockwise && isLeft)
            {
                northDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                northDot.SetAsI();
            }
        }
        if (southDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], southDot);
            if (!clockwise && isLeft)
            {
                southDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                southDot.SetAsI();
            }
        }
    }

    if (pipeMap[i].NodeType == '|')
    {
        if (westDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], westDot);
            if (!clockwise && isLeft)
            {
                westDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                westDot.SetAsI();
            }
        }
        if (eastDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], eastDot);
            if (!clockwise && isLeft)
            {
                eastDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                eastDot.SetAsI();
            }
        }
    }
    
    if (pipeMap[i].NodeType == 'J')
    {
        if (southDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], southDot);
            if (!clockwise && isLeft)
            {
                southDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                southDot.SetAsI();
            }
        }
        if (eastDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], eastDot);
            if (!clockwise && isLeft)
            {
                eastDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                eastDot.SetAsI();
            }
        }
    }
    
    if (pipeMap[i].NodeType == '7')
    {
        if (northDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], northDot);
            if (!clockwise && isLeft)
            {
                northDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                northDot.SetAsI();
            }
        }
        if (eastDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], eastDot);
            if (!clockwise && isLeft)
            {
                eastDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                eastDot.SetAsI();
            }
        }
    }
    
    if (pipeMap[i].NodeType == 'F')
    {
        if (northDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], northDot);
            if (!clockwise && isLeft)
            {
                northDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                northDot.SetAsI();
            }
        }
        if (westDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], westDot);
            if (!clockwise && isLeft)
            {
                westDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                westDot.SetAsI();
            }
        }
    }
    
    if (pipeMap[i].NodeType == 'L')
    {
        if (southDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], southDot);
            if (!clockwise && isLeft)
            {
                southDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                southDot.SetAsI();
            }
        }
        if (westDot != null)
        {
            var isLeft = DetermineIsLeft(pipeMap[i - 1], pipeMap[i], westDot);
            if (!clockwise && isLeft)
            {
                westDot.SetAsI();
            }
            if (clockwise && !isLeft)
            {
                westDot.SetAsI();
            }
        }
    }
}

// Got some random I's in the wrong place, but it's good enough for the flood fill algorithm

// Clean up the edges
foreach (var o in outsidePipeMap.Where(x => x.X == 0 || x.X == XMaxLimit || x.Y == 0 || x.Y == YMaxLimit))
{
    if (o.NodeType is '.' or 'I')
    {
        var oSeed = outsidePipeMap.First(x => x == o);
        FloodFill(outsidePipeMap, oSeed, 'O');
    }
}

foreach (var i in outsidePipeMap.Where(x => x.NodeType == 'I'))
{
    var iSeed = outsidePipeMap.First(x => x == i);
    FloodFill(outsidePipeMap, iSeed, 'I');
}

foreach (var e in outsidePipeMap.Where(x => x.NodeType != 'I' && x.NodeType != 'O'))
{
    // All others that are not an I and O at this point becomes an O
    var eSeed = outsidePipeMap.First(x => x == e);
    FloodFill(outsidePipeMap, eSeed, 'O');
}

Console.WriteLine("Number of I's:" + outsidePipeMap.Count(x => x.NodeType == 'I'));

void FloodFill(List<MapNode> op, MapNode f, char setChar)
{
    switch (setChar)
    {
        case 'O':
            op.First(x => x == f).SetAsO();
            break;
        case 'I':
            op.First(x => x == f).SetAsI();
            break;
    }

    var sNode = op.FirstOrDefault(x => x.X == f.X && x.Y == f.Y + 1 && x.NodeType != setChar);
    var wNode = op.FirstOrDefault(x => x.X == f.X - 1 && x.Y == f.Y && x.NodeType != setChar);
    var eNode = op.FirstOrDefault(x => x.X == f.X + 1 && x.Y == f.Y && x.NodeType != setChar);
    var nNode = op.FirstOrDefault(x => x.X == f.X && x.Y == f.Y - 1 && x.NodeType != setChar);
        
    if(nNode != null)
        FloodFill(op, nNode, setChar);
    if(sNode != null)
        FloodFill(op, sNode, setChar);
    if(wNode != null)
        FloodFill(op, wNode, setChar);
    if(eNode != null)
        FloodFill(op, eNode, setChar);
}

Console.WriteLine("Debug");

bool DetermineIsLeft(MapNode a, MapNode b, MapNode c)
{
    var r = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    return r < 0;
}


*/
