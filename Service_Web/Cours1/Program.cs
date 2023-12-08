class Program
{
    static void Main(string[] args)
    {
        Console.Write("Please enter your name: ");
        string? name = Console.ReadLine();
        
        // Check for null or whitespace to ensure a valid name is entered
        while (string.IsNullOrWhiteSpace(name))
        {
            Console.Write("You must enter a valid name, Please try again: ");
            name = Console.ReadLine();
        }

        Console.Write("Please enter your age : ");
        string? ageInput = Console.ReadLine();
        int age;
        
        // Check for a valid age
        while (!int.TryParse(ageInput, out age))
        {
            Console.Write("That's not a valid age, please enter a valid age: ");
            ageInput = Console.ReadLine();
        }
        
        
        Console.WriteLine("My name is " + name + " and i'm " + age + " years old.");
        Console.WriteLine("My name is {0} and i'm {1} years old.", name, age);
        Console.WriteLine($"My name is {name} and i'm {age} years old.");
    }
}