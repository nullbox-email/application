export default defineAppConfig({
  shadcnDocs: {
    site: {
      name: 'Nullbox',
      description: 'Documentation for Nullbox: aliasing, delivery decisions, and operational guides.',
    },
    theme: {
      customizable: false,
      color: 'zinc',
      radius: 0.5,
    },
    header: {
      title: 'Nullbox',
      showTitle: false,
      darkModeToggle: true,
      languageSwitcher: {
        enable: false,
        triggerType: 'icon',
        dropdownType: 'select',
      },
      logo: {
        light: '/logo.light.svg',
        dark: '/logo.dark.svg',
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
      credits: 'Copyright © 2026',
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
        to: 'https://github.com/nullbox-email/support/issues',
        target: '_blank',
      }],
    },
    search: {
      enable: true,
      inAside: false,
    }
  }
});
