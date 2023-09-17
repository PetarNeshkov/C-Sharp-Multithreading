// Get a dictionary of words.

using System.Diagnostics;

var wordLookup = new HashSet<string>(
    File.ReadAllLines("WordLookup.txt"),
    StringComparer.InvariantCulture);

var random = new Random();
var wordList = wordLookup.ToArray();

// Generate a text from the words.

var wordsToTest = Enumerable.Range(0, 10000000)
    .Select(i => wordList[random.Next(0, wordList.Length)])
    .ToArray();

wordList[12345] = "woozsh";        // Introduce a couple
wordsToTest[23456] = "wubsie";     // of spelling mistakes.

var stopwatch = Stopwatch.StartNew();

var parallelQuery = wordsToTest
    .AsParallel()
    .Select((word, index) => 
        new
        {
            Word = word,
            Index = index
        })
    .Where(word => !wordLookup.Contains(word.Word))
    .OrderBy(word => word.Index)
    .ToList();

Console.WriteLine($"Parallel: {stopwatch.Elapsed}");

stopwatch = Stopwatch.StartNew();

var query = wordsToTest
    .Select((word, index) => 
        new
        {
            Word = word,
            Index = index
        })
    .Where(word => !wordLookup.Contains(word.Word))
    .OrderBy(word => word.Index)
    .ToList();

Console.WriteLine($"Normal: {stopwatch.Elapsed}");

foreach (var result in parallelQuery)
{
    Console.WriteLine(result);
}

foreach (var result in query)
{
    Console.WriteLine(result);
}
