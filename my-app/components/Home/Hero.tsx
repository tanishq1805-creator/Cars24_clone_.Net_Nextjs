"use client";

import { Search } from "lucide-react";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import {
  getCarSuggestions,
  searchCars,
  CarSuggestion,
} from "@/lib/Carapi";
import { Input } from "../ui/input";

const Hero = () => {
  const router = useRouter();

  const [searchQuery, setSearchQuery] = useState("");
  const [suggestions, setSuggestions] = useState<CarSuggestion[]>([]);
  const [cars, setCars] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);

  // Live Suggestions
  useEffect(() => {
    if (searchQuery.trim().length < 2) {
      setSuggestions([]);
      return;
    }

    const timer = setTimeout(async () => {
      try {
        const data = await getCarSuggestions(searchQuery);
        setSuggestions(data);
      } catch (error) {
        console.error(error);
      }
    }, 300);

    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Auto Search
  useEffect(() => {
    if (searchQuery.trim().length < 2) {
      setCars([]);
      return;
    }

    const timer = setTimeout(async () => {
      try {
        setLoading(true);

        const data = await searchCars({
          query: searchQuery,
        });

        setCars(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    }, 500);

    return () => clearTimeout(timer);
  }, [searchQuery]);

  const handleSearch = async () => {
    try {
      setLoading(true);

      const data = await searchCars({
        query: searchQuery,
      });

      setCars(data);
      setSuggestions([]);

      // If exactly one result, go directly to details
      if (data.length === 1) {
        router.push(`/buy-car/${data[0].id}`);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative isolate h-[500px] w-full">
      {/* Background */}
      <div className="absolute inset-0">
        <img
          src="https://images.pexels.com/photos/3802510/pexels-photo-3802510.jpeg"
          alt="Hero"
          className="w-full h-full object-cover"
        />
        <div className="absolute inset-0 bg-gradient-to-r from-black/70 to-black/30" />
      </div>

      {/* Content */}
      <div className="relative z-10 max-w-7xl mx-auto px-4 h-full flex flex-col justify-center">

        <div className="mb-6">
          <h1 className="text-white text-4xl font-bold mb-2">
            Welcome to
            <span className="ml-2">
              <span className="bg-blue-600 text-white px-2 py-1 rounded">
                CARS
              </span>
              <span className="text-orange-500 font-bold ml-1">24</span>
            </span>
          </h1>

          <h2 className="text-white text-5xl font-bold">
            better drives,
          </h2>

          <h2 className="text-white text-5xl font-bold">
            better lives.
          </h2>
        </div>

        {/* Search Box */}
        <div className="bg-white rounded-lg shadow-lg p-5 max-w-4xl w-full">

          <div className="relative">

            <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 h-5 w-5" />

            <Input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  handleSearch();
                }
              }}
              placeholder="Search for Hyundai, Creta, BMW..."
              className="pl-12 pr-4 h-12 border border-gray-300 text-black"
            />

            {/* Suggestions */}
            {suggestions.length > 0 && (
              <div className="absolute left-0 right-0 top-full z-30 mt-2 max-h-80 overflow-y-auto rounded-lg border bg-white shadow-lg">

                {suggestions.map((car) => (
                  <div
                    key={car.id}
                    className="px-4 py-3 hover:bg-gray-100 cursor-pointer border-b last:border-b-0"
                    onClick={() => {
                      setSuggestions([]);
                      router.push(`/buy-car/${car.id}`);
                    }}
                  >
                    <div className="font-semibold text-black">
                      {car.title}
                    </div>

                    <div className="text-sm text-gray-500">
                      {car.year} • {car.fuel} • {car.location}
                    </div>
                  </div>
                ))}

              </div>
            )}
          </div>
        </div>

        {/* Loading */}
        {loading && (
          <p className="text-white mt-4">
            Searching...
          </p>
        )}

        {/* Keep results behind the suggestions dropdown while the user types. */}
        {suggestions.length === 0 && cars.length > 0 && (
          <div className="relative z-10 mt-6 max-w-4xl rounded-lg bg-white p-4 shadow-lg">
            <h2 className="text-xl font-bold mb-4 text-black">
              Search Results
            </h2>

            {cars.map((car) => (
              <div
                key={car.id}
                onClick={() => router.push(`/buy-car/${car.id}`)}
                className="border-b py-3 cursor-pointer hover:bg-gray-50 px-2 rounded"
              >
                <h3 className="font-semibold text-black">
                  {car.title}
                </h3>

                <p className="text-gray-600">
                  ₹ {car.price} • {car.fuel} • {car.transmission}
                </p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Hero;
