using _12._ConsumerProducerPattern;

using var jobs = new ConsumerProducerQueue();

jobs.EnqueueTask("Hello");

for (int i = 0; i < 10; i++)
{
    jobs.EnqueueTask("Say " + i);
}

jobs.EnqueueTask("Goodbye!");