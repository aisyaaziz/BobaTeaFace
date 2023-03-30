namespace BobaTeaFace.Models
{
    public class Course1Detail
    {
        public int Id { get; set; }

        public string Picture { get; set; }

        public int Course1Id { get; set; }
        public virtual Course1 Course1 { get; set; }
    }
}
