export default defineAppConfig({
  shadcnDocs: {
    site: {
      name: 'Nullbox Email',
      description: 'Documentation for Nullbox Email: aliasing, delivery decisions, and operational guides.',
    },
    theme: {
      customizable: true,
      color: 'zinc',
      radius: 0.5,
    },
    header: {
      title: 'Nullbox Email',
      showTitle: true,
      darkModeToggle: true,
      languageSwitcher: {
        enable: false,
        triggerType: 'icon',
        dropdownType: 'select',
      },
      logo: {
        light: '/logo.svg',
        dark: '/logo-dark.svg',
      },
      nav: [],
      links: [{
        icon: 'lucide:github',
        to: 'https://github.com/nullbox-email/application',
        target: '_blank',
      }],
    },
    aside: {
      useLevel: true,
      collapse: false,
    },
    main: {
      breadCrumb: true,
      showTitle: true,
    },
    footer: {
      credits: 'Copyright © 2025',
      links: [{
        icon: 'lucide:github',
        to: 'https://github.com/nullbox-email/application',
        target: '_blank',
      }],
    },
    toc: {
      enable: true,
      links: [{
        title: 'GitHub',
        icon: 'lucide:github',
        to: 'https://github.com/nullbox-email/application',
        target: '_blank',
      }, {
        title: 'Create Issues',
        icon: 'lucide:circle-dot',
        to: 'https://github.com/nullbox-email/application/issues',
        target: '_blank',
      }],
    },
    search: {
      enable: true,
      inAside: false,
    }
  }
});
