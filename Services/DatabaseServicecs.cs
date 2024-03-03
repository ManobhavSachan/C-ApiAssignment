using Microsoft.EntityFrameworkCore;
using Try_pls.Data;
using Try_pls.Models;

namespace Try_pls.Services
{
    
    
        public class DatabaseServicecs
        {
            private readonly ContactsdBcontext dbContext;

            public DatabaseServicecs(ContactsdBcontext dbContext)
            {
                this.dbContext = dbContext;
            }

        public async Task<bool> WriteToDatabaseWithRetry(Func<Task> writeOperation, int maxRetryAttempts = 3)
        {
            int retryCount = 0;

            while (retryCount < maxRetryAttempts)
            {
                try
                {
                    // Execute the provided delegate (writeOperation) representing the database write
                    await writeOperation();

                    return true; // Operation succeeded
                }
                catch (DbUpdateException)
                {
                    // Log the exception or relevant information for debugging purposes
                    LogRetryAttempt(retryCount);

                    retryCount++;
                    await Task.Delay(GetRetryDelay(retryCount));
                }
            }

            // Operation failed after max retry attempts
            return false;
        }

        private void LogRetryAttempt(int attemptNumber)
            {
                // Log relevant information about each retry attempt (e.g., attempt number, timestamp)
                // You can use a logging framework like Serilog or the built-in ILogger.
                Console.WriteLine($"Retry attempt #{attemptNumber + 1} at {DateTime.UtcNow}");
            }

            private TimeSpan GetRetryDelay(int attemptNumber)
            {
                // Implement your backoff strategy (e.g., exponential backoff)
                // Adjust parameters based on your understanding of an appropriate strategy
                int baseDelayMilliseconds = 1000; // 1 second
                int maxDelayMilliseconds = 3000; // 3 seconds

                int delayMilliseconds = Math.Min(baseDelayMilliseconds * (int)Math.Pow(2, attemptNumber), maxDelayMilliseconds);

                return TimeSpan.FromMilliseconds(delayMilliseconds);
            }
        }

    }

