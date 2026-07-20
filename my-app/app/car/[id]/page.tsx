import { redirect } from "next/navigation";

type LegacyCarPageProps = {
  params: Promise<{ id: string }>;
};

// Supports previously generated /car/:id links while the canonical route is
// /buy-car/:id.
export default async function LegacyCarPage({ params }: LegacyCarPageProps) {
  const { id } = await params;
  redirect(`/buy-car/${id}`);
}
