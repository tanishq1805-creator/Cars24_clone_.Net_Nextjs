import Image from "next/image";
import { Geist, Geist_Mono } from "next/font/google";
import Hero from "@/components/Home/Hero";
import Quickaction from "@/components/Home/Quickaction";
import CarBrands from "@/components/Home/CarBrands";
import AppPromotion from "@/components/Home/AppPromotion";
import ServiceCards from "@/components/Home/ServiceCards";
import CarCategories from "@/components/Home/CarCatagories";
import FeaturedCars from "@/components/Home/FeaturedCars";
import CustomerReviews from "@/components/Home/CustomerReviews";

export default function Home() {
  return (
    <div className="flex flex-col w-full bg-white">
      <Hero />
      <div className="max-w-7xl mx-auto px-1 sm:px-1 lg:px-1 w-full bg-white">
        <Quickaction />
        <CarBrands />
        <AppPromotion />
        <ServiceCards />
        <CarCategories />
        <FeaturedCars />
        <CustomerReviews />
      </div>
    </div>
  );
}