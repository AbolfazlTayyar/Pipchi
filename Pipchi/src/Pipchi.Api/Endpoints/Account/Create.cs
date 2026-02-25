using FastEndpoints;
using Pipchi.Api.Models.Account;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace Pipchi.Api.AccountEndpoints
{
    public class Create : Endpoint<CreateAccountRequest, CreateAccountResponse>
    {
        private readonly IRepository<Account> _repository;
        private readonly IMapper _mapper;

        public Create(IRepository<Account> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override void Configure()
        {
            Post(CreateAccountRequest.Route);
            AllowAnonymous();
            Description(x =>
                x.WithSummary("Creates a new Account")
                .WithDescription("Creates a new Account")
                .WithName("accounts.create")
                .WithTags("AccountEndpoints"));
        }

        public override async Task<CreateAccountResponse> ExecuteAsync(CreateAccountRequest request,
            CancellationToken cancellationToken)
        {
            var response = new CreateAccountResponse(request.CorrelationId);

            var balance = new Money(request.Balance, request.Currency);

            var account = await _repository.AddAsync(new Account(Guid.NewGuid(), balance), cancellationToken);

            response.AccountDto = _mapper.Map<AccountDto>(account);

            return response;
        }
    }
}