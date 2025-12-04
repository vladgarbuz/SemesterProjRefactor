using ProjectAurora.Data;
using ProjectAurora.Domain;

namespace ProjectAurora.Data.Rooms
{
    public class ControlRoom : Room
    {
        public ControlRoom(string id, string name, string description) : base(id, name, description) { }

        public override bool OnUseItem(Item item, Player player, GameState state, GameEngine engine)
        {
            var itemName = item.Name.ToLower();
            if (itemName == "lever")
            {
                state.MarkLeverRepaired();
                engine.Print("Lever repaired.");
                return true;
            }
            if (itemName == "berries" || itemName == "pinecone")
            {
                if (player.HasItem("berries") && player.HasItem("pinecone"))
                {
                    if (state.LeverRepaired)
                    {
                        engine.Print("With the lever repaired and derusted, the dam is back to full power!");
                        state.MarkQteComplete();
                        return true;
                    }
                    else
                    {
                            if (engine.RunHydroQTE != null)
                            {
                                var qte = new ProjectAurora.Domain.HydroQte();
                                bool success = engine.RunHydroQTE(qte);
                            if (success)
                            {
                                state.MarkQteComplete();
                                engine.Print("Hydroelectric Dam back to full power!");
                                return true;
                            }
                            else
                            {
                                engine.Print("Failure. You wait 10 seconds...");
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    engine.Print("You need both berries and pinecone to make the acid.");
                    return true;
                }
            }

            return false;
        }
    }
}
