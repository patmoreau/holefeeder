using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

namespace Holefeeder.Application.Features.Categories.Queries.GetCategories;

internal record Request : IRequest<QueryResult<CategoryViewModel>>;
