using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace WatchMate_API.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();

        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entity = await _dbSet.ToListAsync();
            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }
            return entity;
        }

        public async Task<IEnumerable<T>> GetByCompanyIdAsync(string CompanyId)
        {
            // Assuming CompanyId is a property in your entity class
            return await _dbSet.Where(e => EF.Property<string>(e, "CompanyId") == CompanyId).ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }
            return entity;
        }

        public async Task UpdateByForeignKeyAsync<T>(Expression<Func<T, bool>> predicate, Action<T> updateAction) where T : class
        {
            var entity = await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);

            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }

            // Apply the update action
            updateAction(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }
            return entity;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);

        }

        public async Task<T> AddAsyncReturn(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }
        public async Task<T> GetByCustomerIdWithForeignKeysAsync<T>(string customerId, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            IQueryable<T> query = (IQueryable<T>)_dbSet;

            // Dynamically include related entities based on the provided includeProperties
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            // Ensure you're querying the correct navigation property, not just a simple field
            var entity = await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "CustomerId") == customerId);

            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }

            return entity;
        }



        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

        }

        public async Task UpdateAsync(T entity, string propertyName, object value)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

            var property = typeof(T).GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'.");

            if (!property.CanWrite)
                throw new ArgumentException($"Property '{propertyName}' is read-only and cannot be updated.");

            // Attach entity to the context if not tracked
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            // Set property value dynamically and mark it as modified
            property.SetValue(entity, value);
            entry.Property(propertyName).IsModified = true;
        }


        //public async Task<EMIResultDTO> CalculateEMIAsync(decimal loanAmount, decimal annualInterestRate, int tenureMonths)
        //{
        //    return await Task.Run(() =>
        //    {
        //        decimal monthlyInterestRate = annualInterestRate / 100 / 12;
        //        decimal emi, totalInterest, totalPayable;

        //        if (monthlyInterestRate > 0)
        //        {
        //            emi = (loanAmount * monthlyInterestRate *
        //                  (decimal)Math.Pow((double)(1 + monthlyInterestRate), tenureMonths)) /
        //                 ((decimal)Math.Pow((double)(1 + monthlyInterestRate), tenureMonths) - 1);

        //            totalPayable = emi * tenureMonths;
        //            totalInterest = totalPayable - loanAmount;
        //        }
        //        else
        //        {
        //            emi = loanAmount / tenureMonths;
        //            totalInterest = 0;
        //            totalPayable = loanAmount;
        //        }

        //        return new EMIResultDTO
        //        {
        //            MonthlyInstallment = Math.Round(emi, 2),
        //            TotalInterest = Math.Round(totalInterest, 2),
        //            TotalPayable = Math.Round(totalPayable, 2)
        //        };
        //    });
        //}
        public async Task SoftDeleteAsync(int id, int deletedBy)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }

            var propertyInfo = entity.GetType().GetProperty("Deleted");
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(entity, true);
            }

            var deletedByProperty = entity.GetType().GetProperty("DeletedBy");
            if (deletedByProperty != null)
            {
                deletedByProperty.SetValue(entity, deletedBy);
            }

            var deletedAtProperty = entity.GetType().GetProperty("DeletedAt");
            if (deletedAtProperty != null)
            {
                deletedAtProperty.SetValue(entity, DateTime.Now);
            }

            _dbSet.Update(entity);
        }


        public async Task DeleteAsync(int id)
        {
            var entitys = await _dbSet.FindAsync(id);
            if (entitys == null)
            {
                throw new KeyNotFoundException("Entity not found.");
            }
            var softDeleteEnabled = IsSoftDeleteEnabled(entitys.GetType());
            if (softDeleteEnabled)
            {

                var propertyInfo = entitys.GetType().GetProperty("IsDeleted");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(entitys, true);
                }

                var deletedByProperty = entitys.GetType().GetProperty("DeletedBy");
                if (deletedByProperty != null)
                {
                    deletedByProperty.SetValue(entitys, 1);
                }

                var deletedAtProperty = entitys.GetType().GetProperty("DeletedAt");
                if (deletedAtProperty != null)
                {
                    deletedAtProperty.SetValue(entitys, DateTime.Now);
                }

                _dbSet.Update(entitys);
            }
            else
            {
                //Hard Delete
                _dbSet.Remove(entitys);
            }



        }

        public async Task<IEnumerable<T>> GetAllWithOutDeletedAsync()
        {
            var query = _dbSet.AsQueryable();

            var deletedProperty = typeof(T).GetProperty("Deleted");
            if (deletedProperty != null && deletedProperty.PropertyType == typeof(bool?))
            {

                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, deletedProperty);
                var isNull = Expression.Equal(propertyAccess, Expression.Constant(null, typeof(bool?)));
                var isFalse = Expression.Equal(propertyAccess, Expression.Constant(false, typeof(bool?)));
                var filterExpression = Expression.OrElse(isNull, isFalse);
                var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
                query = query.Where(lambda);
            }

            var entities = await query.ToListAsync();
            return entities;
        }

        private bool IsSoftDeleteEnabled(Type entityType)
        {
            // Check if the entity type has the SoftDeleteAttribute
            return entityType.GetCustomAttribute<SoftDeleteAttribute>() != null;
        }

        public async Task<string> SaveDocumentsListsAsync(List<string> documentsBase64, string id, string companyId, string documentType)
        {
            if (documentsBase64 == null || documentsBase64.Count == 0)
                throw new ArgumentException("No documents provided.");

            var documentNames = new List<string>();
            var allowedExtensions = new[] { "jpg", "jpeg", "png", "gif", "pdf" };
            var companyFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", companyId);

            if (!Directory.Exists(companyFolderPath))
                Directory.CreateDirectory(companyFolderPath);

            var documentFolderPath = Path.Combine(companyFolderPath, documentType);
            if (!Directory.Exists(documentFolderPath))
                Directory.CreateDirectory(documentFolderPath);

            // Delete all existing images for the given ID
            var existingFiles = Directory.GetFiles(documentFolderPath, $"{id}_*.*")
                                         .Concat(Directory.GetFiles(documentFolderPath, $"{id}.*"))
                                         .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower().TrimStart('.')));

            foreach (var existingFile in existingFiles)
            {
                System.IO.File.Delete(existingFile);
            }

            int counter = 1;
            foreach (var base64File in documentsBase64)
            {
                var fileType = base64File.Substring(0, base64File.IndexOf(","));
                var fileExtension = fileType.Substring(fileType.IndexOf("/") + 1, fileType.IndexOf(";") - fileType.IndexOf("/") - 1);

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    throw new ArgumentException($"Invalid file type {fileExtension}. Only JPG, JPEG, gif, pdf and PNG are allowed.");

                // If only 1 file, name it as "id.fileExtension"; otherwise, use "id_1.fileExtension", "id_2.fileExtension", etc.
                var fileName = documentsBase64.Count == 1
                    ? $"{id}.{fileExtension}"
                    : $"{id}_{counter++}.{fileExtension}";

                var filePath = Path.Combine(documentFolderPath, fileName);

                var base64Data = base64File.Substring(base64File.IndexOf(",") + 1);
                var fileBytes = Convert.FromBase64String(base64Data);

                if (fileBytes.Length > 2 * 1024 * 1024)
                    throw new InvalidOperationException("File size cannot exceed 2MB.");

                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
                documentNames.Add(fileName);
            }

            return documentsBase64.Count == 1 ? documentNames[0] : JsonConvert.SerializeObject(documentNames);
        }




    }
}
