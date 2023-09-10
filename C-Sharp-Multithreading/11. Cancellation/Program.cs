var tokenSource = new CancellationTokenSource();

var thread = new Thread(() =>
{
   LongOperation(tokenSource.Token);
});

thread.Start();

Console.WriteLine("Enter 'stop' to cancel...");

while (true)
{
   var line = Console.ReadLine();
   if (line?.ToLower() == "stop")
   {
      tokenSource.Cancel();
      break;
   }
}

void LongOperation(CancellationToken cancellationToken)
{
   for (int i = 0; i < 10; i++)
   {
      if (cancellationToken.IsCancellationRequested)
      {
         return;
      }
      
      Thread.Sleep(1000);
      Console.WriteLine("Working...");
   }

   Console.WriteLine("Finished!");
}