const features = [
  {
    title: 'Postgres-ready data layer',
    description:
      'Develop locally against a persistent Postgres instance with sensible defaults and Docker Compose orchestration.'
  },
  {
    title: 'Redis-backed realtime workloads',
    description: 'Redis is provisioned for chat, background jobs, and caching out of the box.'
  },
  {
    title: 'Mail testing sandbox',
    description: 'Mailcatcher collects outbound mail for quick preview while you iterate locally.'
  }
];

export default function HomePage() {
  return (
    <>
      <section className="hero" id="stack">
        <h1 className="hero__headline">Phase 0 Scaffold</h1>
        <p className="hero__lead">
          Kickstart product delivery with a batteries-included Next.js 14 monorepo. Shared tooling, automated
          linting, typed APIs, and locally containerized infrastructure accelerate every team.
        </p>
        <div className="feature-grid" id="services">
          {features.map((feature) => (
            <article key={feature.title} className="feature-card">
              <h2 className="feature-card__title">{feature.title}</h2>
              <p className="feature-card__description">{feature.description}</p>
            </article>
          ))}
        </div>
      </section>
    </>
  );
}
