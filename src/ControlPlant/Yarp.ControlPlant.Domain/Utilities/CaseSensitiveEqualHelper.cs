﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

public static class CaseSensitiveEqualHelper
{
    public static bool Equals(IReadOnlyList<string>? list1, IReadOnlyList<string>? list2)
    {
        return CollectionEqualityHelper.Equals(list1, list2, StringComparer.Ordinal);
    }

    public static bool Equals(IReadOnlyDictionary<string, string>? dictionary1, IReadOnlyDictionary<string, string>? dictionary2)
    {
        return CollectionEqualityHelper.Equals(dictionary1, dictionary2, StringComparer.Ordinal);
    }

    public static bool Equals(
        IReadOnlyList<IReadOnlyDictionary<string, string>>? dictionaryList1,
        IReadOnlyList<IReadOnlyDictionary<string, string>>? dictionaryList2)
    {
        return CollectionEqualityHelper.Equals(dictionaryList1, dictionaryList2, StringComparer.Ordinal);
    }

    public static int GetHashCode(IReadOnlyList<string>? values)
    {
        return CollectionEqualityHelper.GetHashCode(values, StringComparer.Ordinal);
    }

    public static int GetHashCode(IReadOnlyDictionary<string, string>? dictionary)
    {
        return CollectionEqualityHelper.GetHashCode(dictionary, StringComparer.Ordinal);
    }

    public static int GetHashCode(IReadOnlyList<IReadOnlyDictionary<string, string>>? dictionaryList)
    {
        return CollectionEqualityHelper.GetHashCode(dictionaryList);
    }
}