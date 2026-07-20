"use client";

import { useAuth } from "@/context/AuthContext";
import { getReferral, getWallet, getWalletTransactions, Referral, Wallet, WalletTransaction } from "@/lib/userapi";
import { Bell, Calendar, Car, History, LogOut, Mail, Settings, User, WalletCards } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import React, { useEffect, useState } from "react";

export default function ProfilePage() {
  const { user, signOut } = useAuth();
  const router = useRouter();
  const [wallet, setWallet] = useState<Wallet | null>(null);
  const [transactions, setTransactions] = useState<WalletTransaction[]>([]);
  const [walletError, setWalletError] = useState("");
  const [walletLoading, setWalletLoading] = useState(true);
  const [referral, setReferral] = useState<Referral>({
    referralCode: "",
    successfulReferrals: 0,
    walletPoints: 0,
  });

  useEffect(() => {
    if (!user) return;

    const loadWallet = async () => {
      try {
        const [walletData, transactionData, referralData] = await Promise.all([
          getWallet(user.id),
          getWalletTransactions(user.id),
          getReferral(user.id),
        ]);
        setWallet(walletData);
        setTransactions(transactionData);
        setReferral(referralData);
      } catch (error) {
        setWalletError(error instanceof Error ? error.message : "Failed to load wallet");
      } finally {
        setWalletLoading(false);
      }
    };

    loadWallet();
  }, [user]);

  if (!user) {
    return (
      <div className="min-h-screen bg-gray-50 text-black flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600 mb-4">Please log in to view your profile.</p>
          <Link
            href="/login"
            className="inline-block rounded-lg bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
          >
            Go to Login
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 text-black">
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-4xl mx-auto">
          <div className="bg-white rounded-lg shadow-md overflow-hidden">
            <div className="bg-blue-600 px-6 py-8">
              <div className="flex items-center space-x-4">
                <div className="w-20 h-20 rounded-full bg-white flex items-center justify-center">
                  <User className="w-12 h-12 text-blue-600" />
                </div>
                <div>
                  <h1 className="text-2xl font-bold text-white">
                    {user.fullName}
                  </h1>
                  <p className="text-blue-100">{user.email}</p>
                </div>
              </div>
            </div>

            <div className="p-6">
              <section className="mb-8 rounded-lg border border-gray-200 p-5">
                <div className="flex items-center justify-between gap-4">
                  <div className="flex items-center gap-2">
                    <WalletCards className="h-6 w-6 text-blue-600" />
                    <h2 className="text-xl font-semibold">Wallet</h2>
                  </div>
                  <span className="text-xl font-bold text-blue-600">
                    {walletLoading ? "Loading..." : `${wallet?.balance ?? 0} points`}
                  </span>
                </div>

                <div className="mt-5 border-t border-gray-100 pt-4">
                  <div className="mb-3 flex items-center gap-2">
                    <History className="h-5 w-5 text-gray-500" />
                    <h3 className="font-semibold">Transaction History</h3>
                  </div>
                  {walletError ? (
                    <p className="text-sm text-red-600">{walletError}</p>
                  ) : walletLoading ? (
                    <p className="text-sm text-gray-500">Loading transactions...</p>
                  ) : transactions.length === 0 ? (
                    <p className="text-sm text-gray-500">No wallet transactions yet.</p>
                  ) : (
                    <ul className="divide-y divide-gray-100">
                      {transactions.map((transaction) => (
                        <li key={transaction.id} className="flex items-center justify-between gap-4 py-3">
                          <div>
                            <p className="font-medium">{transaction.type}</p>
                            <p className="text-sm text-gray-500">{transaction.description}</p>
                            <p className="text-xs text-gray-400">
                              {new Date(transaction.createdAt).toLocaleString()}
                            </p>
                          </div>
                          <span className={transaction.points >= 0 ? "font-semibold text-green-600" : "font-semibold text-red-600"}>
                            {transaction.points >= 0 ? "+" : ""}{transaction.points} pts
                          </span>
                        </li>
                      ))}
                    </ul>
                  )}
                </div>
              </section>

              <section className="mb-8 rounded-lg border border-gray-200 p-5">
                <h2 className="text-xl font-semibold">Referral</h2>
                <p className="mt-2 text-sm text-gray-600">Your referral code</p>
                <p className="font-mono text-lg font-bold text-blue-600">
                  {walletLoading ? "Loading..." : referral.referralCode || "Not available"}
                </p>
                <p className="mt-2 text-sm text-gray-600">
                  Successful referrals: {referral.successfulReferrals}
                </p>
              </section>
              

              <div className="grid md:grid-cols-3 gap-6">
                <div className="md:col-span-2">
                  <div className="bg-white rounded-lg">
                    <div className="flex justify-between items-center mb-4">
                      <h2 className="text-xl font-semibold">
                        Profile Information
                      </h2>
                    </div>
                    <div className="space-y-4">
                      <div className="flex items-center space-x-2">
                        <User className="w-5 h-5 text-gray-400" />
                        <span className="text-gray-600">Full Name:</span>
                        <span className="font-medium">{user.fullName}</span>
                      </div>
                      <div className="flex items-center space-x-2">
                        <Mail className="w-5 h-5 text-gray-400" />
                        <span className="text-gray-600">Email:</span>
                        <span className="font-medium">{user.email}</span>
                      </div>
                      <div className="flex items-center space-x-2">
                        <Bell className="w-5 h-5 text-gray-400" />
                        <span className="text-gray-600">Phone:</span>
                        <span className="font-medium">{user.phone}</span>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="space-y-4">
                  <h2 className="text-xl font-semibold">Quick Actions</h2>
                  <div className="space-y-2">
                    <button className="w-full flex items-center space-x-2 p-3 text-left rounded-lg hover:bg-gray-50">
                      <Settings className="w-5 h-5 text-gray-400" />
                      <span>Account Settings</span>
                    </button>

                    <button
                      onClick={() => router.push("/bookings")}
                      className="w-full flex items-center space-x-2 p-3 text-left rounded-lg hover:bg-gray-50"
                    >
                      <Car className="w-5 h-5 text-gray-400" />
                      <span>My Cars</span>
                    </button>

                    <button
                      onClick={() => router.push("/appointments")}
                      className="w-full flex items-center space-x-2 p-3 text-left rounded-lg hover:bg-gray-50"
                    >
                      <Calendar className="w-5 h-5 text-gray-400" />
                      <span>Appointments</span>
                    </button>

                    <button
                      onClick={() => {
                        signOut();
                        router.push("/");
                      }}
                      className="w-full flex items-center space-x-2 p-3 text-left rounded-lg hover:bg-gray-50 text-red-600"
                    >
                      <LogOut className="w-5 h-5" />
                      <span>Sign Out</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
