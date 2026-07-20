"use client";

import { X, Star } from "lucide-react";
import { Reward } from "@/lib/rewardAPI";

interface RewardModalProps {
  reward: Reward | null;
  walletPoints: number;
  redeeming: boolean;
  onClose: () => void;
  onRedeem: () => void;
}

export default function RewardModal({
  reward,
  walletPoints,
  redeeming,
  onClose,
  onRedeem,
}: RewardModalProps) {
  if (!reward) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-5">

      <div className="relative w-full max-w-2xl rounded-3xl bg-white shadow-2xl">

        {/* Close */}

        <button
          onClick={onClose}
          className="absolute right-5 top-5 rounded-full bg-gray-100 p-2 hover:bg-gray-200"
        >
          <X size={22} />
        </button>

        {/* Image */}

        <div className="relative h-72 w-full">

          <img
            src={reward.imageUrl || "/reward/Discount.png"}
            alt={reward.title}
            className="h-full w-full rounded-t-3xl object-cover"
          />

        </div>

        {/* Content */}

        <div className="p-8">

          <h2 className="text-3xl font-bold">
            {reward.title}
          </h2>

          <p className="mt-4 text-gray-600">
            {reward.description}
          </p>

          <div className="mt-6 flex items-center gap-2">

            <Star
              className="text-yellow-500"
              fill="currentColor"
            />

            <span className="text-lg font-bold">
              {reward.pointsRequired} Points
            </span>

          </div>

          {/* Terms */}

          <div className="mt-8 rounded-xl bg-gray-100 p-5">

            <h3 className="font-semibold">
              Terms & Conditions
            </h3>

            <ul className="mt-3 list-disc space-y-2 pl-5 text-sm text-gray-600">
              <li>Reward can be redeemed only once.</li>
              <li>Reward is non-transferable.</li>
              <li>Cannot be exchanged for cash.</li>
              <li>Applicable only on eligible bookings.</li>
            </ul>

          </div>

          {/* Button */}

          <button
            onClick={onRedeem}
            disabled={
              redeeming ||
              walletPoints < reward.pointsRequired
            }
            className={`mt-8 w-full rounded-xl py-4 font-semibold transition ${
              walletPoints >= reward.pointsRequired
                ? "bg-blue-600 text-white hover:bg-blue-700"
                : "cursor-not-allowed bg-gray-300 text-gray-500"
            }`}
          >
            {redeeming
              ? "Redeeming..."
              : walletPoints >= reward.pointsRequired
              ? "Redeem Reward"
              : "Not Enough Points"}
          </button>

        </div>

      </div>

    </div>
  );
}
