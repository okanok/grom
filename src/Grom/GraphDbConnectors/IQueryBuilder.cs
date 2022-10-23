using Grom.GromQuery;

namespace Grom.GraphDbConnectors;

interface IQueryBuilder
{
    internal string BuildQuery(IConstraintNode state);
}
