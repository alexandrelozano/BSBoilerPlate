namespace BSBoilerPlate.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
