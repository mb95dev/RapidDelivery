using MediatR;

namespace Common.CQRS;


//public interface IQueryHandler<in TRequest, TResponse>
//     : IRequestHandler<TRequest, TResponse>
//where TRequest : IQuery<TResponse>
//where TResponse : notnull
//{

//}


public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{ }