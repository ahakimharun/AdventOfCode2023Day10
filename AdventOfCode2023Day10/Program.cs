string inputFile = @"C:\Users\SaLiVa\source\repos\AdventOfCode2023Day10\AdventOfCode2023Day10\Day10Input.txt";
string outputFile = @"C:\Users\SaLiVa\source\repos\AdventOfCode2023Day10\AdventOfCode2023Day10\Day10Output.txt";
List<MapNode> map = [];

using (StreamReader reader = File.OpenText(inputFile))
{
    uint x = 0, y = 0;
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
// Find dot clusters (I guess)

var dotMap = map.Where(x => x.NodeType == '.');
// Assumption that all dots at the edge are Os
// They could be seeds for the traversal and transformation of Os
var edgeDots = dotMap
    .Where(x => x.X == 0 || x.Y == 0 || x.X == XMaxLimit -1 || x.Y == YMaxLimit - 1);
foreach (var d in edgeDots)
    d.SetAsO();

// New idea - When walking along the pipe - 'Visit' the dots on the right
// Then walk backwards and 'revisit' the dots on the left
// Those that have been 'visited' twice should be in the loop

// Writer for sanity check
// using (StreamWriter outputWriter = new StreamWriter(outputFile))
// {
//     for (int y = 0; y < YMaxLimit; y++)
//     {
//         string outputString = string.Empty;
//         for (int x = 0; x < XMaxLimit; x++)
//         {
//             var nodeType = pipeMap.FirstOrDefault(p => p.X == x && p.Y == y);
//             char nodeTypeChar = ' ';
//             if(nodeType != null)
//                 nodeTypeChar = nodeType.NodeType;
//             outputString += nodeTypeChar == '.' ? ' ' : nodeTypeChar;
//         }
//         outputWriter.WriteLine(outputString);
//     }
// }



Console.WriteLine("Debug");
    
public class MapNode
{
    public MapNode(uint x, uint y, char nodeType)
    {
        X = x;
        Y = y;
        NodeType = nodeType;
        NumberOfVisits = 0;
        StepsFromStart = 0;
    }

    public uint X { get; }
    public uint Y { get; }
    public char NodeType { get; private set; }

    public void TriggerVisit()
    {
        NumberOfVisits++;
    }

    public void SetAsO()
    {
        NodeType = 'O';
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