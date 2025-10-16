module.exports = {
  root: true,
  extends: ['@phase/eslint-config'],
  ignorePatterns: ['**/node_modules', '**/.next', '**/dist', '**/.turbo'],
  overrides: [
    {
      files: ['apps/web/**/*.{ts,tsx,js,jsx}'],
      extends: ['@phase/eslint-config/next']
    },
    {
      files: [
        '**/__tests__/**/*.{ts,tsx,js,jsx}',
        '**/*.test.{ts,tsx,js,jsx}',
        '**/*.spec.{ts,tsx,js,jsx}'
      ],
      env: {
        jest: true
      }
    }
  ]
};
