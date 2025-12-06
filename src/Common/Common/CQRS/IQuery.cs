using MediatR;

namespace Common.CQRS;

public interface IQuery<TResponse> : IRequest<TResponse> { }
//public interface IQuery<out TResponse> : IRequest<TResponse>
//    where TResponse : notnull
//{

//}

