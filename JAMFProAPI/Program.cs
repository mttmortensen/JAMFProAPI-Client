using JAMFProAPI;

class Program
{
    static async Task Main()
    {
        var calvinJMBA = "196"; // HQS-CALVINJ-MBA
        var users = await FileVault2.GetFileVault2UsersAsync(calvinJMBA);

        if (users != null && users.Count > 0)
        {
            Console.WriteLine("FileVault 2 Users:");
            foreach (var user in users)
            {
                Console.WriteLine(user);
            }
        }
        else
        {
            Console.WriteLine("No FileVault 2 users returned for the specified computer.");
        }
    }
}
