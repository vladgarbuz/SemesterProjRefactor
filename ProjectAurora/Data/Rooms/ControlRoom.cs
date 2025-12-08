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
                // Use lever to repair; consume the lever as it's used
                if (player.HasItem("lever"))
                {
                    player.RemoveItem("lever");
                }
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
                        // Consume berries and pinecone
                        if (player.HasItem("berries"))
                            player.RemoveItem("berries");
                        if (player.HasItem("pinecone"))
                            player.RemoveItem("pinecone");
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
                                // Berries and pinecone are consumed to make the acid
                                if (player.HasItem("berries"))
                                    player.RemoveItem("berries");
                                if (player.HasItem("pinecone"))
                                    player.RemoveItem("pinecone");
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
