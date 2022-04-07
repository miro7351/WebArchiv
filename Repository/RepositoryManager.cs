
//using Entities;
using PA.Contracts;
using PA.TOYOTA.DB;

namespace PA.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
       
        private ToyotaContext _repositoryContext;
        /*
        private IZakazkaRepository _zakazkaRepository;
        private IDokumentRepository _employeeRepository;
        */
        public RepositoryManager(ToyotaContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        /*
        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_repositoryContext);

                return _companyRepository;
            }
        }

        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new EmployeeRepository(_repositoryContext);

                return _employeeRepository;
            }
        }
        */
       
        public void Save() => _repositoryContext.SaveChanges();
        
    }
}