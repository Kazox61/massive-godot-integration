using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive.Netcode;

public class GenericTypeLookup<TType> {
	private readonly Dictionary<Type, int> _typeByIds = new();
	private readonly Dictionary<int, Type> _idByTypes = new();
	private readonly FastList<Type> _types = new();
	
	public void RegisterAll() {
		var inputTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(type => !type.IsAbstract && typeof(TType).IsAssignableFrom(type))
			.OrderBy(type => type.GetFullGenericName())
			.ToArray();

		foreach (var inputType in inputTypes) {
			Register(inputType);
		}
	}
	
	public void Register<T>() where T : TType {
		Register(typeof(T));
	}

	public void Register(Type type) {
		if (!_typeByIds.TryAdd(type, _types.Count)) {
			throw new Exception($"Duplicate input type registration. Type: {type.GetFullGenericName()}");
		}
		
		_idByTypes.Add(_types.Count, type);
		_types.Add(type);
	}

	public Type GetType(int id) {
		return _idByTypes[id];
	}
	
	public int GetTypeId<T>() where T : TType {
		return GetTypeId(typeof(T));
	}
	
	public int GetTypeId(Type type) {
		return _typeByIds[type];
	}
}