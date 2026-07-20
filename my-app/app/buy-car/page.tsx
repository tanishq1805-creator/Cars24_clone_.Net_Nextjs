"use client";

import { Input } from "@/components/ui/input";
import { Slider } from "@/components/ui/slider";
import {
  CarSummary,
  City,
  getCarSuggestions,
  getCities,
  searchCars,
} from "@/lib/Carapi";
import { Heart, Search, Sliders, X } from "lucide-react";
import Link from "next/link";
import { useEffect, useState } from "react";

const brands = ["Maruti", "Hyundai", "Honda", "Tata", "Mahindra", "Toyota", "Kia"];
const currentYear = new Date().getFullYear();

function LoaderCard() {
  return <div className="h-96 animate-pulse rounded-lg bg-gray-200" />;
}

export default function BuyCarPage() {
  const [city, setCity] = useState("Delhi NCR");
  const [cities, setCities] = useState<City[]>([]);
  const [cityLoadError, setCityLoadError] = useState("");
  const [detectingLocation, setDetectingLocation] = useState(false);
  const [userLatitude, setUserLatitude] = useState<number>();
  const [userLongitude, setUserLongitude] = useState<number>();
  const [query, setQuery] = useState("");
  const [suggestions, setSuggestions] = useState<string[]>([]);
  const [priceRange, setPriceRange] = useState([0, 1_000_000]);
  const [mileageRange, setMileageRange] = useState([0, 200_000]);
  const [yearRange, setYearRange] = useState([2005, currentYear]);
  const [selectedBrands, setSelectedBrands] = useState<string[]>([]);
  const [fuel, setFuel] = useState("");
  const [transmission, setTransmission] = useState("");
  const [cars, setCars] = useState<CarSummary[] | null>(null);

  useEffect(() => {
    const loadCities = async () => {
      try {
        const loadedCities = await getCities();
        setCities(loadedCities);

        const savedCity = localStorage.getItem("selectedCity");
        if (savedCity && loadedCities.some((item) => item.name === savedCity)) {
          setCity(savedCity);
        }
      } catch (error) {
        console.error("Unable to load cities:", error);
        setCityLoadError("Unable to load cities. Make sure the API is running.");
      }
    };

    void loadCities();
  }, []);

  useEffect(() => {
    const timer = window.setTimeout(async () => {
      try {
        setCars(null);
        setCars(await searchCars({
          city,
          query,
          brands: selectedBrands,
          fuel,
          transmission,
          minPrice: priceRange[0],
          maxPrice: priceRange[1],
          minMileage: mileageRange[0],
          maxMileage: mileageRange[1],
          minYear: yearRange[0],
          maxYear: yearRange[1],

          userLatitude,
          userLongitude,
        }));
      } catch (error) {
        console.error("Unable to load cars:", error);
        setCars([]);
      }
    }, 250);
    return () => window.clearTimeout(timer);
  }, [city, query, selectedBrands, fuel, transmission, priceRange, mileageRange, yearRange, userLatitude, userLongitude]);

  useEffect(() => {
    const timer = window.setTimeout(async () => {
      const results = await getCarSuggestions(query);
      setSuggestions(results.map((suggestion) => suggestion.title));
    }, 180);
    return () => window.clearTimeout(timer);
  }, [query]);

  const resetFilters = () => {
    setQuery("");
    setSelectedBrands([]);
    setFuel("");
    setTransmission("");
    setPriceRange([0, 1_000_000]);
    setMileageRange([0, 200_000]);
    setYearRange([2005, currentYear]);
  };

  const selectCity = (nextCity: string) => {
    setCity(nextCity);
    localStorage.setItem("selectedCity", nextCity);
  };

  const detectLocation = () => {
    if (!navigator.geolocation) {
      alert("Geolocation is not supported by your browser.");
      return;
    }

    setDetectingLocation(true);

    navigator.geolocation.getCurrentPosition(
      async (position) => {
        const { latitude, longitude } = position.coords;
        setUserLatitude(latitude);
        setUserLongitude(longitude);

        try {
          const response = await fetch(
            `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${latitude}&lon=${longitude}`
          );

          const data = await response.json();

          const detectedCity =
            data.address.city ||
            data.address.town ||
            data.address.village ||
            data.address.state_district ||
            data.address.state;

          if (detectedCity) {
            const matchedCity = cities.find(
              (item) =>
                item.name.toLowerCase() === detectedCity.toLowerCase()
            );

            if (matchedCity) {
              selectCity(matchedCity.name);
            } else {
              alert(
                `Detected city: ${detectedCity}. It's not in our supported city list.`
              );
            }
          }
        } catch (error) {
          console.error(error);
        } finally {
          setDetectingLocation(false);
        }
      },
      () => {
        alert("Unable to detect your location.");
        setDetectingLocation(false);
      }
    );
  };

  const activeFilters =
    selectedBrands.length + Number(Boolean(fuel)) + Number(Boolean(transmission));

  return (
    <main className="min-h-screen bg-gray-100 py-8 text-black">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 gap-6 md:grid-cols-4">
          <aside className="space-y-4 md:col-span-1">
            <div className="rounded-lg bg-white p-4 shadow-sm">
              <div className="mb-5 flex items-center justify-between"><h2 className="font-semibold">Filters</h2><button type="button" onClick={resetFilters} className="text-sm text-blue-700 hover:underline">Clear all</button></div>
              <div className="space-y-6">
                <FilterSlider label="Price range" value={priceRange} max={1_000_000} step={10_000} prefix="Rs. " onChange={setPriceRange} />
                <FilterSlider label="Mileage range" value={mileageRange} max={200_000} step={5_000} suffix=" km" onChange={setMileageRange} />
                <FilterSelect label="Fuel type" value={fuel} onChange={setFuel} options={["Petrol", "Diesel", "CNG", "Electric", "Hybrid"]} />
                <FilterSelect label="Transmission" value={transmission} onChange={setTransmission} options={["Manual", "Auto", "Automatic"]} />
                <div><p className="mb-2 text-sm font-medium">Brand</p><div className="space-y-2">{brands.map((brand) => <label key={brand} className="flex cursor-pointer items-center gap-2 text-sm"><input type="checkbox" checked={selectedBrands.includes(brand)} onChange={() => setSelectedBrands((current) => current.includes(brand) ? current.filter((item) => item !== brand) : [...current, brand])} />{brand}</label>)}</div></div>
              </div>
            </div>
          </aside>
          <section className="md:col-span-3">
            <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <h1 className="text-2xl font-bold">
                  Used Cars in {city}
                </h1>

                <div className="mt-2 inline-flex items-center gap-2 rounded-full bg-blue-100 px-3 py-1 text-sm font-medium text-blue-700">
                  {city}
                </div>

                <p className="mt-2 text-sm text-gray-600">
                  Ranked by relevance, popularity and listing freshness.
                </p>
              </div>
              <div className="flex w-full flex-col gap-3 md:flex-row md:items-center">


                {/* City Selector */}
                <div className="relative min-w-[220px]">
                  <div className="mb-2 flex items-center gap-2 text-xs font-medium text-gray-500">
                    📍 Current Location
                  </div>

                  <select
                    value={city}
                    onChange={(e) => selectCity(e.target.value)}
                  >
                    {cities.map(city => (
                      <option
                        key={city.id}
                        value={city.name}
                      >
                        {city.name}, {city.state}
                      </option>
                    ))}
                  </select>
                  {cityLoadError && (
                    <p className="mt-2 text-xs text-red-600">{cityLoadError}</p>
                  )}

                  <button
                    type="button"
                    onClick={detectLocation}
                    disabled={detectingLocation}
                    className="mt-2 w-full rounded-lg bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-60"
                  >
                    {detectingLocation ? "Detecting..." : "📍 Use My Location"}
                  </button>
                </div>
                {/* Search Bar */}
                <div className="relative flex-1">
                  <Search className="pointer-events-none absolute left-3 top-3 h-4 w-4 text-gray-400" />

                  <Input
                    value={query}
                    onChange={(event) => setQuery(event.target.value)}
                    placeholder="Search make, model or location"
                    className="h-11 pl-10 pr-9"
                  />

                  {query && (
                    <button
                      type="button"
                      onClick={() => setQuery("")}
                      className="absolute right-2 top-2 rounded p-1 hover:bg-gray-100"
                    >
                      <X className="h-4 w-4" />
                    </button>
                  )}

                  {suggestions.length > 0 && (
                    <div className="absolute z-20 mt-1 w-full overflow-hidden rounded-lg border bg-white shadow-lg">
                      {suggestions.map((suggestion) => (
                        <button
                          key={suggestion}
                          type="button"
                          onClick={() => {
                            setQuery(suggestion);
                            setSuggestions([]);
                          }}
                          className="block w-full px-4 py-2 text-left hover:bg-gray-100"
                        >
                          {suggestion}
                        </button>
                      ))}
                    </div>
                  )}
                </div>

              </div>
            </div>
            <div className="mb-4 flex items-center justify-between text-sm text-gray-600"><span>{cars === null ? "Searching listings..." : `${cars.length} cars found`}</span><span className="inline-flex items-center gap-1"><Sliders className="h-4 w-4" />{activeFilters ? `${activeFilters} quick filters active` : "All filters"}</span></div>
            {cars?.length === 0 ? <div className="rounded-lg bg-white p-10 text-center text-gray-600">No cars match these filters. Try widening your search.</div> : <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">{cars === null ? Array.from({ length: 6 }, (_, index) => <LoaderCard key={index} />) : cars.map((car) => <CarCard key={car.id} car={car} />)}</div>}
          </section>
        </div>
      </div>
    </main>
  );
}

function FilterSlider({ label, value, max, step, prefix = "", suffix = "", onChange }: { label: string; value: number[]; max: number; step: number; prefix?: string; suffix?: string; onChange: (value: number[]) => void }) {
  return <div><label className="mb-2 block text-sm font-medium">{label}</label><Slider value={value} max={max} step={step} onValueChange={(next) => onChange(Array.isArray(next) ? [...next] : [next, next])} /><div className="mt-2 flex justify-between text-xs text-gray-600"><span>{prefix}{value[0].toLocaleString("en-IN")}{suffix}</span><span>{prefix}{value[1].toLocaleString("en-IN")}{suffix}</span></div></div>;
}

function FilterSelect({ label, value, onChange, options }: { label: string; value: string; onChange: (value: string) => void; options: string[] }) {
  return <div><label className="mb-2 block text-sm font-medium">{label}</label><select value={value} onChange={(event) => onChange(event.target.value)} className="h-9 w-full rounded border border-gray-300 bg-white px-2 text-sm"><option value="">Any {label.toLowerCase()}</option>{options.map((option) => <option key={option} value={option}>{option}</option>)}</select></div>;
}

function CarCard({ car }: { car: CarSummary }) {
  return <Link href={`/buy-car/${car.id}`} className="overflow-hidden rounded-lg bg-white shadow-sm transition-shadow hover:shadow-md">
    <div className="relative h-48 bg-gray-100">
      {car.image && <img src={car.image} alt={car.title} className="h-full w-full object-cover" />}
      <button type="button" aria-label={`Save ${car.title}`} className="absolute right-2 top-2 rounded-full bg-white/90 p-2" onClick={(event) => event.preventDefault()}>
      <Heart className="h-4 w-4 text-gray-600" /></button></div>
      <div className="p-4">
        <h2 className="mb-2 line-clamp-2 font-semibold">{car.title}</h2>
        <p className="mb-3 text-sm text-gray-600">{car.year} | {car.km} km | {car.fuel} | {car.transmission}</p>
        <div className="flex items-end justify-between">
          <div><p className="text-xs text-gray-500">EMI from</p><p className="font-semibold">{car.emi}</p></div>
          <div className="text-right">
    <p className="text-xs text-gray-500">Base Price</p>

    <p className="font-semibold">
        {car.price}
    </p>

    {car.recommendedPrice != null && (
        <>
            <p className="mt-2 text-xs text-gray-500">
                Recommended Price
            </p>

            <p className="font-semibold text-green-600">
                ₹{car.recommendedPrice.toLocaleString("en-IN")}
            </p>

        </>
    )}

    {car.pricingReason && (
        <p className="text-[11px] text-gray-500">
            {car.pricingReason}
        </p>
    )}
</div>
  </div>
    <p className="mt-3 truncate text-xs text-gray-500">{car.location}</p>{car.distance != null && (
      <p className="mt-2 text-xs font-medium text-blue-600">
        📍 {car.distance.toFixed(1)} km away
      </p>
    )}</div></Link>;
}
