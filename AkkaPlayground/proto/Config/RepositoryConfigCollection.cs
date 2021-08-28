using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Util;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Config
{
    public class RepositoryConfigCollection : List<RepositoryConfig>
    {
        public Result<int> CheckIntegrity()
        {
            var errors = new List<string>();
            if (this.All(c => c.Reader == null))
            {
                errors.Add("No reader present");
            }
            if (this.All(c => c.Reader == null))
            {
                errors.Add("No reader present");
            }

            var rwCollision =
                this.Where(c =>
                        c.Writer?.FieldsMask.IsMatch(
                            c.Reader?.FieldsMask ?? new FieldsMask()
                        )
                        ?? false
                    )
                    .Select(c => c.Info.Name)
                    .ToList();
            if (rwCollision.Any())
            {
                errors.Add($"Read/Write collision: {string.Join(",", rwCollision)}");
            }

            var rCollision =
                this.Where(r =>
                        this.Any(c =>
                            r.Info.Name != c.Info.Name &&
                            (
                                r.Reader?.FieldsMask.IsMatch(
                                    c.Reader?.FieldsMask ?? new FieldsMask()
                                )
                                ?? false
                            )
                        )
                    )
                    .Select(c => c.Info.Name)
                    .ToList();
            if (rCollision.Any())
            {
                errors.Add($"Read collision: {string.Join(",", rCollision)}");
            }

            var nameCollision =
                this
                    .Select(r => r.Info.Name)
                    .GroupBy(n => n)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
            if (nameCollision.Any())
            {
                errors.Add($"Name collision: {string.Join(",", nameCollision)}");
            }

            if (errors.Any())
            {
                return new Result<int>(
                    new Exception(string.Join("\n", errors))
                );
            }
            return new Result<int>(0);
        }
    }
}