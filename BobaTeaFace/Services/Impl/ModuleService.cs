using BobaTeaFace.Data;
using BobaTeaFace.Models;

namespace BobaTeaFace.Services.Impl
{
    public class ModuleService : IModuleService
    {
        private ApplicationDbContext _db;

        public ModuleService(ApplicationDbContext db)
        {
            _db = db;
        }

        public IQueryable<Course> getCourses()
        {
            return _db.Courses;
        }
    }
}
