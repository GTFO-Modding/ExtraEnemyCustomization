﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EEC.Utils.Json.Converters
{
    public sealed class Vector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vector = new Vector2();

            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            return vector;

                        if (reader.TokenType != JsonTokenType.PropertyName)
                            throw new JsonException("Expected PropertyName token");

                        var propName = reader.GetString();
                        reader.Read();

                        switch (propName.ToLowerInvariant())
                        {
                            case "x":
                                vector.x = reader.GetSingle();
                                break;

                            case "y":
                                vector.y = reader.GetSingle();
                                break;
                        }
                    }
                    throw new JsonException("Expected EndObject token");

                case JsonTokenType.String:
                    var strValue = reader.GetString().Trim();
                    if (TryParseVector2(strValue, out vector))
                    {
                        return vector;
                    }
                    throw new JsonException($"Vector2 format is not right: {strValue}");

                default:
                    throw new JsonException($"Vector2Json type: {reader.TokenType} is not implemented!");
            }
        }

        private static bool TryParseVector2(string input, out Vector2 vector)
        {
            if (!RegexUtil.TryParseVectorString(input, out var array))
            {
                vector = Vector2.zero;
                return false;
            }

            if (array.Length < 2)
            {
                vector = Vector2.zero;
                return false;
            }

            vector = new Vector2(array[0], array[1]);
            return true;
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(string.Format("({0} {1})", value.x, value.y));
        }
    }
}