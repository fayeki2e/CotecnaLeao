using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Data
{
    public partial class ApplicationEntityFrameworkRepository
        : EntityFrameworkRepository<ApplicationDbContext>, IApplicationRepository
    {
        public ApplicationEntityFrameworkRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
