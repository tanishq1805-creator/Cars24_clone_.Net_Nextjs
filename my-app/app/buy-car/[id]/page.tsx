"use client";

import { Input } from "@/components/ui/input";
import { useAuth } from "@/context/AuthContext";
import { createBooking } from "@/lib/Bookingapi";
import { CarDetails, getCarById } from "@/lib/Carapi";
import { getVehicleCare, VehicleCareResponse } from "@/lib/VehicleCareApi";
import {
  AlertCircle,
  Calendar,
  Car,
  Clock,
  CreditCard,
  MapPin,
  Phone,
  User,
} from "lucide-react";
import { useParams, useRouter } from "next/navigation";
import React, { useEffect, useState } from "react";
import { toast } from "sonner";

export default function CarDetailPage() {
  const [formData, setformData] = useState({
    name: "",
    phone: "",
    email: "",
    address: "",
    preferredDate: "",
    preferredTime: "",
    paymentMethod: "",
    loanRequired: "no",
    downPayment: "",
  });
  const router = useRouter();
  const params = useParams();
  const id = typeof params.id === "string" ? params.id : "";
  const { user } = useAuth();
  const [carDetails, setcarDetails] = useState<CarDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [step, setstep] = useState(1);
  const [vehicleCare, setVehicleCare] = useState<VehicleCareResponse["data"] | null>(null);

  useEffect(() => {
    async function fetchCar() {
      try {
        const selectedCity = localStorage.getItem("selectedCity") ?? "";

        const data = await getCarById(id, selectedCity);
        setcarDetails(data);
        const care = await getVehicleCare(id);
        setVehicleCare(care.data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    }
    fetchCar();
  }, [id]);

  if (loading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-10 w-10 border-t-2 border-b-2 border-gray-900" />
      </div>
    );
  }
  if (!carDetails) {
    return (
      <div className="text-center mt-10 text-red-500">
        Car details not found.
      </div>
    );
  }
  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setformData((Prev) => ({
      ...Prev,
      [name]: value,
    }));
  };
  const handlesubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!user) {
      toast.error("Please login to continue");
      return;
    }
    try {
      const booking = {
        CarId: id,
        name: formData.name,
        phone: formData.phone,
        email: formData.email,
        address: formData.address,
        preferredDate: formData.preferredDate,
        preferredTime: formData.preferredTime,
        paymentMethod: formData.paymentMethod,
        loanRequired: formData.loanRequired,
        downPayment: formData.downPayment,
      };
      const response = await createBooking(user.id, booking);
      if (response.id) {
        toast.success("Bookings listed Successfully");
        router.push(`/bookings`);
      }
    } catch (error) {
      console.error(error);
      toast.error("Failed to complete booking");
    }
  };

  const validatestep = () => {
    if (step === 1) {
      return formData.name && formData.phone && formData.email;
    }
    if (step === 2) {
      return formData.preferredDate && formData.preferredTime;
    }
    return true;
  };
  const availableTimes = [
    "10:00 AM",
    "11:00 AM",
    "12:00 PM",
    "2:00 PM",
    "3:00 PM",
    "4:00 PM",
  ];



  return (
    <div className="min-h-screen bg-gray-50 text-black">
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-7xl mx-auto">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            {/* Car Details Summary */}
            <div className="bg-white rounded-lg shadow-md p-6">
              {/* Car Image */}
              <div className="aspect-w-16 aspect-h-9 mb-4">
                <img
                  src={carDetails.images[0]}
                  alt={carDetails.title}
                  className="w-full h-64 object-cover rounded-lg"
                />
              </div>

              {/* Title and Price */}
              <h2 className="text-2xl font-bold mb-2">{carDetails.title}</h2>
              <p className="text-sm text-gray-400 mb-4">ID: {carDetails.id}</p>

              <div>
                <p className="text-sm text-gray-500">
                  Base Price
                </p>

                <p className="text-3xl font-bold text-blue-600">
                  {carDetails.price}
                </p>

                {carDetails.recommendedPrice != null && (
                  <>
                    <p className="mt-3 text-sm text-gray-500">
                      Recommended Price
                    </p>

                    <p className="text-2xl font-bold text-green-600">
                      ₹{carDetails.recommendedPrice.toLocaleString("en-IN")}
                    </p>

                  </>
                )}

                {carDetails.pricingReason && (
                  <p className="text-sm text-gray-500">
                    {carDetails.pricingReason}
                  </p>
                )}

                <p className="mt-2 text-gray-600">
                  EMI from {carDetails.emi}
                </p>
              </div>
              {/* Specs Grid */}
              <div className="grid grid-cols-2 gap-4 mb-6">
                <div className="bg-gray-50 p-3 rounded-lg">
                  <p className="text-gray-600">Year</p>
                  <p className="font-medium">{carDetails.specs.year}</p>
                </div>
                <div className="bg-gray-50 p-3 rounded-lg">
                  <p className="text-gray-600">Fuel Type</p>
                  <p className="font-medium">{carDetails.specs.fuel}</p>
                </div>
                <div className="bg-gray-50 p-3 rounded-lg">
                  <p className="text-gray-600">Transmission</p>
                  <p className="font-medium">{carDetails.specs.transmission}</p>
                </div>
                <div className="bg-gray-50 p-3 rounded-lg">
                  <p className="text-gray-600">Owner</p>
                  <p className="font-medium">{carDetails.specs.owner}</p>
                </div>
                <div className="bg-gray-50 p-3 rounded-lg">
                  <p className="text-gray-600">Insurance</p>
                  <p className="font-medium">{carDetails.specs.insurance}</p>
                </div>
              </div>
              {/* Highlights */}
              <div className="bg-blue-50 p-4 rounded-lg mb-4">
                <h3 className="font-semibold text-blue-800 mb-2">
                  Car Highlights
                </h3>
                <ul className="space-y-1">
                  {carDetails.highlights.map((highlight, index) => (
                    <li key={index} className="text-blue-700 flex items-center">
                      <div className="w-2 h-2 bg-blue-700 rounded-full mr-2"></div>
                      {highlight}
                    </li>
                  ))}
                </ul>
              </div>
              {/* Features */}
              <div className="bg-gray-100 p-4 rounded-lg">
                <h3 className="font-semibold text-gray-800 mb-2">Features</h3>
                <ul className="list-disc list-inside space-y-1 text-gray-700">
                  {carDetails.features.map((feature, index) => (
                    <li key={index}>{feature}</li>
                  ))}
                </ul>
              </div>
            </div>

            {vehicleCare && (
              <div className="bg-green-50 border border-green-200 rounded-lg p-5 mt-6">
                <h3 className="text-xl font-bold text-green-700 mb-4">
                  Vehicle Care
                </h3>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-gray-500 text-sm">Condition</p>
                    <p className="font-semibold">{vehicleCare.condition}</p>
                  </div>

                  <div>
                    <p className="text-gray-500 text-sm">Monthly Cost</p>
                    <p className="font-semibold">
                      ₹{vehicleCare.monthlyCost}
                    </p>
                  </div>

                  <div>
                    <p className="text-gray-500 text-sm">Annual Cost</p>
                    <p className="font-semibold">
                      ₹{vehicleCare.annualCost}
                    </p>
                  </div>

                  <div>
                    <p className="text-gray-500 text-sm">Next Service</p>
                    <p className="font-semibold">
                      {vehicleCare.nextServiceInKm} km
                    </p>
                  </div>
                </div>

                <div className="mt-5">
                  <h4 className="font-semibold mb-2">
                    Recommended Maintenance
                  </h4>

                  <ul className="list-disc list-inside space-y-1">
                    {vehicleCare.predictions.map(
                      (item: string, index: number) => (
                        <li key={index}>{item}</li>
                      )
                    )}
                  </ul>
                </div>
              </div>
            )}

            {/* booking form  */}
            <div className="bg-white rounded-lg shadow-md p-6">
              <h2 className="text-2xl font-bold mb-6">
                Complete Your Purchase
              </h2>

              <div className="mb-6">
                <div className="flex items-center space-x-4">
                  {[1, 2, 3].map((stepNumber) => (
                    <div key={stepNumber} className="flex items-center">
                      <div
                        className={`w-8 h-8 rounded-full flex items-center justify-center ${step === stepNumber
                          ? "bg-blue-600 text-white"
                          : step > stepNumber
                            ? "bg-green-500 text-white"
                            : "bg-gray-200 text-gray-600"
                          }`}
                      >
                        {step > stepNumber ? "✓" : stepNumber}
                      </div>
                      {stepNumber < 3 && (
                        <div
                          className={`w-12 h-1 ${step > stepNumber ? "bg-green-500" : "bg-gray-200"
                            }`}
                        ></div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
              <form onSubmit={handlesubmit} className="space-y-6">
                {step === 1 && (
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        <User className="w-4 h-4 inline mr-1" /> Full Name
                      </label>
                      <input
                        type="text"
                        name="name"
                        value={formData.name}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        <Phone className="w-4 h-4 inline mr-1" /> Phone Number
                      </label>
                      <input
                        type="tel"
                        name="phone"
                        value={formData.phone}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Email Address
                      </label>
                      <input
                        type="email"
                        name="email"
                        value={formData.email}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      />
                    </div>
                  </div>
                )}
                {step === 2 && (
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        <Calendar className="w-4 h-4 inline mr-1" /> Preferred
                        Visit Date
                      </label>
                      <Input
                        type="date"
                        name="preferredDate"
                        value={formData.preferredDate}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        <Clock className="w-4 h-4 inline mr-1" /> Preferred Time
                      </label>
                      <select
                        name="preferredTime"
                        value={formData.preferredTime}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      >
                        <option value="">Select a time</option>
                        {availableTimes.map((time) => (
                          <option key={time} value={time}>
                            {time}
                          </option>
                        ))}
                      </select>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        <MapPin className="w-4 h-4 inline mr-1" /> Address
                      </label>
                      <input
                        type="text"
                        name="address"
                        value={formData.address}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      />
                    </div>
                  </div>
                )}
                {step === 3 && (
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        <CreditCard className="w-4 h-4 inline mr-1" /> Payment
                        Method
                      </label>
                      <select
                        name="paymentMethod"
                        value={formData.paymentMethod}
                        onChange={handleInputChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        required
                      >
                        <option value="">Select payment method</option>
                        <option value="full">Full Payment</option>
                        <option value="loan">Car Loan</option>
                      </select>
                    </div>
                    {formData.paymentMethod === "loan" && (
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                          Down Payment Amount
                        </label>
                        <input
                          type="text"
                          name="downPayment"
                          value={formData.downPayment}
                          onChange={handleInputChange}
                          placeholder="Enter amount"
                          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                        />
                      </div>
                    )}

                    <div className="bg-yellow-50 p-4 rounded-lg">
                      <div className="flex items-start">
                        <AlertCircle className="w-5 h-5 text-yellow-600 mt-0.5 mr-2" />
                        <div>
                          <h4 className="text-sm font-medium text-yellow-800">
                            Required Documents
                          </h4>
                          <ul className="mt-2 text-sm text-yellow-700 list-disc list-inside">
                            <li>Valid ID Proof</li>
                            <li>Address Proof</li>
                            <li>Income Proof (for loan)</li>
                            <li>Bank Statements (for loan)</li>
                          </ul>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                <div className="flex justify-between pt-6">
                  {step > 1 && (
                    <button
                      type="button"
                      onClick={() => setstep(step - 1)}
                      className="px-4 py-2 text-gray-600 border border-gray-300 rounded-md hover:bg-gray-50"
                    >
                      Back
                    </button>
                  )}

                  {step < 3 ? (
                    <button
                      type="button"
                      onClick={() => validatestep() && setstep(step + 1)}
                      className={`px-6 py-2 rounded-md text-white ${validatestep()
                        ? "bg-blue-600 hover:bg-blue-700"
                        : "bg-gray-400 cursor-not-allowed"
                        }`}
                      disabled={!validatestep()}
                    >
                      Continue
                    </button>
                  ) : (
                    <button
                      type="submit"
                      className="px-6 py-2 bg-green-600 text-white rounded-md hover:bg-green-700"
                    >
                      Complete Purchase
                    </button>
                  )}
                </div>
              </form>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
