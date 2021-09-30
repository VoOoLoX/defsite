using System.Collections.Generic;
using System.Threading;
using Common;

namespace Server;

public abstract class Handler {
	protected List<Client> Clients = new();

	bool start_fixed_updates = true;

	AutoResetEvent fixed_update_state = new(false);
	object update_clients_lock = new();

	protected virtual int TicksPerSecond => 20;

	void UpdateClients(List<Client> client_list) {
		lock(update_clients_lock) {
			Clients = client_list;
		}
	}

	protected virtual void Update() { }

	protected virtual void FixedUpdate(object state) { }

	public void Run(bool fixed_time_step = false) {
		UpdateClients(Server.GetClients());

		if(fixed_time_step && start_fixed_updates) {
			start_fixed_updates = false;
			var _ = new Timer(FixedUpdate, fixed_update_state, 0, 1000 / TicksPerSecond);
			// now = DateTime.Now.Millisecond;
			// FixedUpdate(delta_update_time);
			// delta_update_time = DateTime.Now.Millisecond - now;
			// if (delta_update_time < 1000 / TPS) Thread.Sleep(1000 / TPS - (int)delta_update_time);
		}
		Update();

	}
}
