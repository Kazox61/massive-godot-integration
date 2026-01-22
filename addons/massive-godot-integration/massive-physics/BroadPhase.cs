using System;
using System.Collections.Generic;
using Fixed;
using Fixed32;

namespace Massive.Physics;

public class BroadPhase {
	private readonly DynamicTree[] _trees = [
		new(), // Static
		new(), // Kinematic
		new() // Dynamic
	];

	private readonly List<int> _moveBuffer = [];
	private readonly HashSet<long> _pairSet = [];

	private readonly List<Pair> _pairs = [];

	private static int MakeProxyKey(int proxyId, BodyType type) => (proxyId << 2) | (int)type;

	private static int ProxyId(int key) => key >> 2;

	private static BodyType ProxyType(int key) => (BodyType)(key & 0b11);

	public int CreateProxy(
		BodyType type,
		FAABB aabb,
		ulong categoryBits,
		ulong userData
	) {
		var proxyId = _trees[(int)type].CreateProxy(aabb, categoryBits, userData);
		var proxyKey = MakeProxyKey(proxyId, type);

		if (type != BodyType.Static) {
			BufferMove(proxyKey);
		}

		return proxyKey;
	}

	public void DestroyProxy(int proxyKey) {
		_moveBuffer.Remove(proxyKey);

		var type = ProxyType(proxyKey);
		var id = ProxyId(proxyKey);

		_trees[(int)type].DestroyProxy(id);
	}

	public void MoveProxy(int proxyKey, FAABB aabb) {
		var type = ProxyType(proxyKey);
		var id = ProxyId(proxyKey);

		_trees[(int)type].MoveProxy(id, aabb);
		BufferMove(proxyKey);
	}


	private void BufferMove(int proxyId) {
		_moveBuffer.Add(proxyId);
	}

	private bool PairQueryCallback(int proxyId, ulong otherUserData, object contextObj) {
		var context = (QueryContext)contextObj;
		var queryProxyKey = context.QueryProxyKey;

		var queryType = ProxyType(queryProxyKey);
		var queryId = ProxyId(queryProxyKey);
		var queryUserData = _trees[(int)queryType].Nodes[queryId].UserData;

		if (queryUserData == otherUserData) {
			return true;
		}

		var key = ((long)queryUserData << 32) | (uint)otherUserData;
		if (!_pairSet.Add(key)) return true;

		_pairs.Add(new Pair { AUserData = queryUserData, BUserData = otherUserData });
		return true;
	}



	public void UpdatePairs(Action<ulong, ulong> onPair) {
		_pairs.Clear();
		_pairSet.Clear();

		foreach (var queryProxyKey in _moveBuffer) {
			var queryType = ProxyType(queryProxyKey);
			var queryId = ProxyId(queryProxyKey);

			var tree = _trees[(int)queryType];
			var fatAABB = tree.Nodes[queryId].AABB;

			QueryTree(BodyType.Dynamic, queryProxyKey, fatAABB);

			if (queryType == BodyType.Dynamic) {
				QueryTree(BodyType.Kinematic, queryProxyKey, fatAABB);
				QueryTree(BodyType.Static, queryProxyKey, fatAABB);
			}
		}

		foreach (var pair in _pairs) {
			onPair(pair.AUserData, pair.BUserData);
		}

		_moveBuffer.Clear();
	}
	
	private void QueryTree(
		BodyType targetType,
		int queryProxyKey,
		FAABB aabb
	) {
		var context = new QueryContext {
			QueryProxyKey = queryProxyKey,
			TargetType = targetType
		};

		_trees[(int)targetType].Query(
			aabb,
			ulong.MaxValue,
			PairQueryCallback,
			context
		);
	}


	private struct Pair {
		public ulong AUserData;
		public ulong BUserData;
	}

	public enum BodyType {
		Static = 0,
		Kinematic = 1,
		Dynamic = 2
	}
	
	private struct QueryContext {
		public int QueryProxyKey;
		public BodyType TargetType;
	}
}