using MediatR;

namespace SkillMap.Business;
public interface ICommand<out TResult> : IRequest<TResult>
{ }

public interface ICommand : IRequest
{ }