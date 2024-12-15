using Dealytics.Domain.Dtos;
using Dealytics.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dealytics.Domain.Mappers
{
    public static class ActionDTOMapper
    {
        public static ActionDTO ToDto(this Entities.Action action)
        {
            var actionDto = new ActionDTO
            {
                Name = action.Id,
                Positions = new List<Position>()
            };

            if (action.Positions != null)
            {
                foreach (var item in action.Positions)
                {
                    actionDto.Positions.Add(item);
                }
            }

            return actionDto;
        }
        public static Entities.Action ToEntity(this ActionDTO action)
        {
            var actionEntity = new Entities.Action
            {
                Id = action.Name,
                Positions = new List<Position>()
            };
            if (action.Positions != null)
            {
                foreach (var item in action.Positions)
                {
                    actionEntity.Positions.Add(item);
                }
            }
            return actionEntity;
        }

    }
}
