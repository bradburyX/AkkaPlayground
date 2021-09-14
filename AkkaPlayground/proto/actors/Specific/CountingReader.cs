using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using AkkaPlayground.Proto.Actors.Factory;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Masking;

namespace AkkaPlayground.Proto.Actors.Specific
{
    [ActorFor(Network.Read, RepositoryType.Counting)]
    public class CountingReader : ConfiguredActor
    {
        private int i;

        public CountingReader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            Receive<Reader.DoWorkCycle>(
                _ =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Context.Parent.Tell(
                            new ChangeSet(j.ToString(), CreateRow())
                        );
                        i++;
                    }
                });
        }

        private List<Field> CreateRow()
        {
            return new List<Field>
            {
                new Field
                {
                    Col = FieldName.Name,
                    Val = "Name_6"
                },
                new Field
                {
                    Col = FieldName.Email,
                    Val = "Email_5"
                },
                new Field
                {
                    Col = FieldName.City,
                    Val = "City_6"
                },
            };
            
            return 
                WorkerConfig
                    .Fields
                    .Select(f => 
                        new Field
                        {
                            Col = f,
                            Val = $"{f}_{i}"
                        }
                    )
                    .ToList();
        }
    }
}
