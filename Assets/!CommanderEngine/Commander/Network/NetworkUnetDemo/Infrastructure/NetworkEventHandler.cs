using UnityEngine;
using UnityEngine.Networking;

namespace NetworkUnetDemo.Infrastructure
{
  /// <summary>
  /// Every frame, trigger any pending network events and redirect them to the appropriate targets.
  /// If this component is missing, nothing will run.
  /// </summary>
  public class NetworkEventHandler : MonoBehaviour
  {
    [Tooltip("The connection manager to delegate events through")]
    public NetworkHostManager HostManager;

    void Awake()
    {
      // Any application level configuration here.
      var config = new GlobalConfig();
      NetworkTransport.Init(config);
    }

    //void Update()
    //{
            // This happens twice, HostManager has Update as well
     // HostManager.Update();
    //}
  }
}