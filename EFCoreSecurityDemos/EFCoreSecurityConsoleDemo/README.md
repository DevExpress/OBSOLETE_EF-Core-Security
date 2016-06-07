This solution demonstrates how to work with DbContext directly in a console application:

            using(EFCoreDemoDbContext dbContext = new EFCoreDemoDbContext()) {
                Console.WriteLine("Username: ");
                string userName = Console.ReadLine();
                Console.WriteLine("Please enter password: ");
                string password = Console.ReadLine();
                try {
                    dbContext.Logon(userName, password);
                    IEnumerable<Contact> contacts = dbContext.Contacts.Include(c => c.ContactTasks).ThenInclude(ct => ct.Task).Include(c => c.Department);
                    foreach(Contact contact in contacts) {
                      Console.WriteLine("\n{0}. Name: {1}\nAddress: {2}", i, contact.Name, contact.Address);
                      ...
                    }
                }
                catch(Exception exception) {
                    Console.WriteLine(exception.Message);
                }
            }

