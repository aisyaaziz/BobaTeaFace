using BobaTeaFace.Models;

namespace BobaTeaFace.Services
{
    public interface IModuleService
    {
        public IQueryable<Course> getCourses();
    }
}
