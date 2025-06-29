using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using FluentValidation;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductDto> _validator;

        public ProductService(IUnitOfWork uow, IMapper mapper, IValidator<CreateProductDto> validator)
        {
            _uow = uow;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _uow.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

            var product = _mapper.Map<Product>(dto);
            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product is not null)
            {
                _uow.Products.Delete(product);
                await _uow.SaveChangesAsync();
            }
        }
    }
}
