"use client";

import { useEffect, useMemo, useState } from "react";
import {
    Gift,
    Search,
    Star,
    Wallet,
    Sparkles,
} from "lucide-react";

import { useAuth } from "@/context/AuthContext";
import { getReferral, getWallet } from "@/lib/userapi";
import { toast } from "sonner";
import {
    Reward,
    getRewards,
    redeemReward,
} from "@/lib/rewardAPI";
import RewardModal from "@/components/RewardModal";


const categories = [
    "All",
    "Discount",
    "Service",
    "Membership",
    "Voucher",
];

export default function RewardStorePage() {
    const { user } = useAuth();

    const [walletPoints, setWalletPoints] = useState(0);

    const [rewards, setRewards] = useState<Reward[]>([]);

    const [loading, setLoading] = useState(true);

    const [redeemingId, setRedeemingId] = useState("");

    const [selectedCategory, setSelectedCategory] =
        useState("All");

    const [search, setSearch] = useState("");
    const [selectedReward, setSelectedReward] = useState<Reward | null>(null);

    useEffect(() => {
        if (!user) return;

        loadData();
    }, [user]);

    async function loadData() {
        try {
            const [wallet, rewardData] = await Promise.all([
                getWallet(user!.id),
                getRewards(),
            ]);

            setWalletPoints(wallet.balance);

            setRewards(rewardData);
        } catch (err) {
            console.log(err);
        } finally {
            setLoading(false);
        }
    }

    async function handleRedeem(reward: Reward) {
        if (!user) return;

        try {
            setRedeemingId(reward.id);

            await redeemReward(user.id, reward.id);

            alert("Reward Redeemed Successfully!");

            loadData();
        } catch (err: any) {
            toast.error(err.message);
        } finally {
            setRedeemingId("");
        }
    }

    async function handleInviteFriends() {
        if (!user) {
            toast.error("Please log in to copy your referral code.");
            return;
        }

        try {
            const { referralCode } = await getReferral(user.id);

            if (!referralCode) {
                toast.error("Your referral code is not available yet.");
                return;
            }

            await navigator.clipboard.writeText(referralCode);
            toast.success("Referral code copied to your clipboard.");
        } catch (error) {
            console.error("Unable to copy referral code:", error);
            toast.error("Unable to copy your referral code. Please try again.");
        }
    }

    const filteredRewards = useMemo(() => {
        return rewards.filter((reward) => {
            const categoryMatch =
                selectedCategory === "All" ||
                reward.category === selectedCategory;

            const searchMatch =
                reward.title
                    .toLowerCase()
                    .includes(search.toLowerCase()) ||
                reward.description
                    .toLowerCase()
                    .includes(search.toLowerCase());

            return categoryMatch && searchMatch;
        });
    }, [rewards, selectedCategory, search]);

    if (loading)
        return (
            <div className="p-10 text-center">
                Loading Reward Store...
            </div>
        );

    return (
        <div className="min-h-screen bg-[#0b0b0d] text-white">

            {/* Hero */}

            <div className="bg-gradient-to-r from-blue-700 to-indigo-700">

                <div className="max-w-7xl mx-auto px-8 py-12">

                    <div className="flex items-center gap-3">

                        <Gift className="w-12 h-12 text-yellow-300" />

                        <div>

                            <h1 className="text-5xl font-bold">
                                Reward Store
                            </h1>

                            <p className="text-blue-100 mt-2">
                                Redeem your reward points for exciting
                                Cars24 benefits.
                            </p>

                        </div>

                    </div>

                    {/* Wallet */}

                    <div className="mt-10 rounded-2xl bg-white text-black p-8 shadow-xl">

                        <div className="flex items-center justify-between">

                            <div>

                                <p className="text-gray-500">
                                    Available Balance
                                </p>

                                <h2 className="text-5xl font-bold text-blue-600 mt-2">

                                    {walletPoints}

                                </h2>

                                <p className="font-semibold">
                                    Reward Points
                                </p>

                            </div>

                            <div className="hidden md:flex">

                                <Wallet className="w-24 h-24 text-blue-500" />

                            </div>

                        </div>

                        {/* Progress */}

                        <div className="mt-8">

                            <div className="flex justify-between text-sm mb-2">

                                <span>Reward Progress</span>

                                <span>Next Reward at 500 Points</span>

                            </div>

                            <div className="h-3 rounded-full bg-gray-200">

                                <div
                                    className="h-3 rounded-full bg-gradient-to-r from-yellow-400 to-orange-500"
                                    style={{
                                        width: `${Math.min(
                                            (walletPoints / 500) * 100,
                                            100
                                        )}%`,
                                    }}
                                />

                            </div>

                        </div>

                    </div>

                </div>

            </div>

            {/* Search */}

            <div className="max-w-7xl mx-auto px-8 mt-10">

                <div className="relative">

                    <Search className="absolute left-5 top-4 text-gray-500" />

                    <input
                        type="text"
                        placeholder="Search Rewards..."
                        value={search}
                        onChange={(e) =>
                            setSearch(e.target.value)
                        }
                        className="w-full rounded-xl bg-white text-black pl-14 pr-4 py-4 outline-none shadow-lg"
                    />

                </div>

                {/* Categories */}

                <div className="flex gap-4 flex-wrap mt-8">

                    {categories.map((category) => (

                        <button
                            key={category}
                            onClick={() =>
                                setSelectedCategory(category)
                            }
                            className={`px-6 py-3 rounded-full font-semibold transition ${selectedCategory === category
                                ? "bg-blue-600"
                                : "bg-white text-black hover:bg-gray-200"
                                }`}
                        >

                            {category}

                        </button>

                    ))}

                </div>

                {/* Cards Start */}

                <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8 mt-10">
                    {filteredRewards.map((reward) => (
                        <div
                            key={reward.id}
                            onClick={(e) => {
                                e.stopPropagation();
                                setSelectedReward(reward);
                            }}
                            className="group cursor-pointer overflow-hidden rounded-2xl bg-white text-black shadow-xl transition-all duration-300 hover:-translate-y-2 hover:shadow-2xl"
                        >
                            {/* Reward Image */}

                            <div className="relative h-52 overflow-hidden bg-gradient-to-br from-blue-100 via-white to-orange-100">

                                <img
                                    src={
                                        reward.imageUrl || "/reward/Discount.png"
                                    }
                                    alt={reward.title}
                                    className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-110"
                                />

                                {/* Category Badge */}

                                <span className="absolute left-4 top-4 rounded-full bg-blue-600 px-3 py-1 text-xs font-semibold text-white shadow">

                                    {reward.category}

                                </span>

                                {/* Featured Badge */}

                                <div className="absolute right-4 top-4 rounded-full bg-yellow-400 p-2 shadow-lg">

                                    <Sparkles className="h-4 w-4 text-white" />

                                </div>

                            </div>

                            {/* Card Content */}

                            <div className="p-6">

                                <h2 className="line-clamp-2 text-xl font-bold">

                                    {reward.title}

                                </h2>

                                <p className="mt-3 line-clamp-3 text-sm text-gray-600">

                                    {reward.description}

                                </p>

                                {/* Points */}

                                <div className="mt-5 flex items-center justify-between">

                                    <div className="flex items-center gap-2 rounded-full bg-yellow-100 px-4 py-2">

                                        <Star
                                            className="h-5 w-5 text-yellow-500"
                                            fill="currentColor"
                                        />

                                        <span className="font-bold text-yellow-700">

                                            {reward.pointsRequired}

                                        </span>

                                        <span className="text-sm text-gray-600">

                                            Points

                                        </span>

                                    </div>

                                    {walletPoints >= reward.pointsRequired ? (
                                        <span className="rounded-full bg-green-100 px-3 py-1 text-xs font-semibold text-green-700">

                                            Eligible

                                        </span>
                                    ) : (
                                        <span className="rounded-full bg-red-100 px-3 py-1 text-xs font-semibold text-red-700">

                                            Need {reward.pointsRequired - walletPoints}

                                        </span>
                                    )}

                                </div>

                                {/* Redeem Button */}

                                <button
                                    onClick={() => handleRedeem(reward)}
                                    disabled={
                                        walletPoints < reward.pointsRequired ||
                                        redeemingId === reward.id
                                    }
                                    className={`mt-6 w-full rounded-xl py-3 font-semibold transition-all duration-300 ${walletPoints >= reward.pointsRequired
                                        ? "bg-blue-600 text-white hover:bg-blue-700"
                                        : "cursor-not-allowed bg-gray-300 text-gray-500"
                                        }`}
                                >
                                    {redeemingId === reward.id
                                        ? "Redeeming..."
                                        : walletPoints >= reward.pointsRequired
                                            ? "Redeem Reward"
                                            : "Not Enough Points"}
                                </button>
                            </div>
                        </div>
                    ))}

                </div>

                {/* Empty State */}

                {filteredRewards.length === 0 && (
                    <div className="mt-20 rounded-2xl bg-white p-16 text-center shadow-xl">
                        <Gift className="mx-auto h-16 w-16 text-gray-400" />

                        <h2 className="mt-6 text-2xl font-bold text-gray-800">
                            No Rewards Found
                        </h2>

                        <p className="mt-3 text-gray-500">
                            Try searching with a different keyword or category.
                        </p>
                    </div>
                )}

                {/* Earn More Points */}

                <div className="mt-20 overflow-hidden rounded-3xl bg-gradient-to-r from-orange-500 via-red-500 to-pink-600">

                    <div className="flex flex-col items-center justify-between gap-8 p-12 text-center lg:flex-row lg:text-left">

                        <div>

                            <h2 className="text-4xl font-bold text-white">
                                Earn More Reward Points
                            </h2>

                            <p className="mt-4 max-w-2xl text-lg text-orange-100">
                                Invite your friends to Cars24 and earn reward points every
                                time they successfully purchase a vehicle using your
                                referral code.
                            </p>

                            <button
                                type="button"
                                onClick={handleInviteFriends}
                                className="mt-8 rounded-xl bg-white px-8 py-3 font-semibold text-orange-600 transition hover:scale-105"
                            >
                                Invite Friends
                            </button>

                        </div>

                        <Gift className="h-40 w-40 text-white opacity-90" />

                    </div>

                </div>

                {/* Reward Tips */}

                <div className="mt-16 grid gap-8 md:grid-cols-3">

                    <div className="rounded-2xl bg-white p-8 text-black shadow-lg">

                        <Gift className="mb-4 h-10 w-10 text-blue-600" />

                        <h3 className="text-xl font-bold">
                            Redeem Rewards
                        </h3>

                        <p className="mt-3 text-gray-600">
                            Use your points to unlock discounts, services and premium
                            membership.
                        </p>

                    </div>

                    <div className="rounded-2xl bg-white p-8 text-black shadow-lg">

                        <Wallet className="mb-4 h-10 w-10 text-green-600" />

                        <h3 className="text-xl font-bold">
                            Save More
                        </h3>

                        <p className="mt-3 text-gray-600">
                            Redeem your wallet points during booking and reduce your
                            purchase cost.
                        </p>

                    </div>

                    <div className="rounded-2xl bg-white p-8 text-black shadow-lg">

                        <Sparkles className="mb-4 h-10 w-10 text-yellow-500" />

                        <h3 className="text-xl font-bold">
                            Exclusive Benefits
                        </h3>

                        <p className="mt-3 text-gray-600">
                            Premium members receive exclusive Cars24 offers and priority
                            services.
                        </p>

                    </div>

                </div>

            </div>

            {/* Footer */}

            <footer className="mt-24 border-t border-gray-800 py-10">

                <div className="text-center text-gray-400">

                    <p className="text-lg font-medium">
                        🚗 Cars24 Reward Store
                    </p>

                    <p className="mt-2">
                        Redeem • Save • Enjoy Exclusive Benefits
                    </p>

                </div>

            </footer>

            <RewardModal
                reward={selectedReward}
                walletPoints={walletPoints}
                redeeming={redeemingId === selectedReward?.id}
                onClose={() => setSelectedReward(null)}
                onRedeem={() => {
                    if (selectedReward) {
                        handleRedeem(selectedReward);
                        setSelectedReward(null);
                    }
                }}
            />

        </div>


    );
}
