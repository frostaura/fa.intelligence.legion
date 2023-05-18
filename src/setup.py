from setuptools import setup, find_packages
from pathlib import Path

ROOT_DIR = Path(__file__).parent
VERSION = '0.0.1'
DESCRIPTION = 'FrostAura Zeus is an autonomous general purpose, multi-modal agent.'
LONG_DESCRIPTION = DESCRIPTION #(ROOT_DIR / '../README.md').read_text()

setup(
    name='frostaura.intelligence.zeus',
    version=VERSION,
    author='Dean Martin',
    author_email='dean.martin@frostaura.net',
    description=DESCRIPTION,
    long_description=LONG_DESCRIPTION,
    long_description_content_type='text/markdown',
    url='https://github.com/faGH/fa.intelligence.notebooks',
    packages=find_packages(),
    install_requires=[],
    keywords=[
        'frostaura',
        'deep learning',
        'machine learning',
        'bots',
        'agi',
        'ai',
        'llm'
    ],
    include_package_data=True,
    classifiers=[
        'Programming Language :: Python :: 3.7',
        'Programming Language :: Python :: 3.8',
        'Programming Language :: Python :: 3.9',
        'License :: OSI Approved :: MIT License',
        'Operating System :: OS Independent'
    ],
    python_requires='>=3.7'
)
